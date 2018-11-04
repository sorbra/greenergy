using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using greenergy.chatbot_fulfillment.Models;
using Greenergy.API;
using Microsoft.Extensions.Logging;
using Greenergy.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace greenergy.chatbot_fulfillment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private const float kilometersPerKwh = 5f;
        private const float kwhPerHour = 10f;
        private const string ChargeCarYESIntent = "projects/greenergy-3dbfe/agent/intents/07a5c59b-1bdc-4dc5-8788-5380b48c5324";
        private const string ChargeCarNOIntent = "projects/greenergy-3dbfe/agent/intents/63256d8b-9a44-4dd3-829f-47dc8073bc8c";
        private const string ConsumeElectricityIntent = "projects/greenergy-3dbfe/agent/intents/6e9a8963-9a74-4343-8e0d-a7b2563db55c";
        private const string CurrentCo2QueryIntent = "projects/greenergy-3dbfe/agent/intents/460c430c-52a7-4313-8e23-20bb9e191bf0";
        private const string SendReminderIntent = "projects/greenergy-3dbfe/agent/intents/2f19e6a9-a5c9-404c-a4e0-4d34f90edeba";
        private const string DrivesomewhereintentFollowupContext = "drivesomewhereintent-followup";
        private const string ConsumeElectricityOutputContext = "consume-electricity-output-context";

        private IGreenergyAPI _greenergyAPI;
        private ILogger<FulfillmentController> _logger;
        private IOptions<FulfillmentSettings> _config;
        private TimeZoneInfo _copenhagenTimeZoneInfo;

        public FulfillmentController(IGreenergyAPI greenergyAPI, ILogger<FulfillmentController> logger, IOptions<FulfillmentSettings> config)
        {
            _greenergyAPI = greenergyAPI;
            _logger = logger;
            _config = config;

            try
            {
                _copenhagenTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_config.Value.TimeZone);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<EmissionDataDTO>>> Getemissions()
        {
            try
            {
                return await _greenergyAPI.GetMostRecentEmissions();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<DialogFlowResponseDTO>> Fulfill([FromBody] DialogFlowRequestDTO request)
        {
            try
            {
                var intent = request.queryResult.intent.name;
                if (intent.Equals(ChargeCarYESIntent))
                {
                    return await HandleChargeCarIntent(request, true);
                }
                else if (intent.Equals(ChargeCarNOIntent))
                {
                    return await HandleChargeCarIntent(request, false);
                }
                else if (intent.Equals(ConsumeElectricityIntent))
                {
                    return await HandleConsumeElectricityIntent(request);
                }
                else if (intent.Equals(SendReminderIntent))
                {
                    return await HandleSendReminderIntent(request);
                }
                else if (intent.Equals(CurrentCo2QueryIntent))
                    return await HandleCurrentCo2QueryIntent(request);
                else
                {
                    _logger.LogError($"FulfillmentController.Fulfill: Unknown intent: {request.queryResult.intent.displayName}");
                    return NotFound();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in FulfillmentController.Fulfill", null);
                return null;
            }
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleCurrentCo2QueryIntent(DialogFlowRequestDTO request)
        {
            try
            {
                var currentEmission = (await _greenergyAPI.GetMostRecentEmissions())[0].Emission;

                DialogFlowResponseDTO response = new DialogFlowResponseDTO();
                response.fulfillmentText = request.queryResult.fulfillmentText.Replace("$co2perkwh", currentEmission.ToString());

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleSendReminderIntent(DialogFlowRequestDTO request)
        {
            try
            {
                var response = new DialogFlowResponseDTO();
                response.outputContexts = request.queryResult.outputContexts;
                response.fulfillmentText = request.queryResult.fulfillmentText;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleConsumeElectricityIntent(DialogFlowRequestDTO request)
        {
            try
            {
                Parameters parameters = request.queryResult.outputContexts
                            .FirstOrDefault(oc => oc.name.EndsWith(ConsumeElectricityOutputContext))
                            .parameters;

                var best = await _greenergyAPI.OptimalFutureConsumptionTime(
                    consumptionMinutes: parameters.duration.toMinutes(),
                    consumptionRegion: "DK1",
                    startNoEarlierThan: DateTime.Now,
                    finishNoLaterThan: DateTime.MaxValue
                );

                if (best != null)
                {
                    DialogFlowResponseDTO response = new DialogFlowResponseDTO();
                    var consumptionStart = TimeZoneInfo.ConvertTime(best.consumptionStart, _copenhagenTimeZoneInfo);
    
                    int savingsPercentage = (best.optimalCo2perkwh - best.currentCo2perkwh) / best.currentCo2perkwh;

                    response.fulfillmentText = request.queryResult.fulfillmentText
                                .Replace("$co2perkwh", best.optimalCo2perkwh.ToString())
                                .Replace("$optimal-time", consumptionStart.ToShortTimeString())
                                .Replace("$optimal-day", consumptionStart.DayOfWeek.ToString())
                                .Replace("$savings-percentage", savingsPercentage.ToString());
                    return response;
                }
                else
                {
                    _logger.LogError("Received null return value from OptimalFutureConsumptionTime");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleChargeCarIntent(DialogFlowRequestDTO request, Boolean doCharge)
        {
            try
            {
                var parameters = request.queryResult.outputContexts
                                        .FirstOrDefault(oc => oc.name.EndsWith(DrivesomewhereintentFollowupContext))
                                        .parameters;

                DateTime driveTime = parameters.time.ToUniversalTime();
                DateTime driveDate = parameters.time.Date;

                float hoursNeeded = parameters.kilometers / kilometersPerKwh / kwhPerHour;

                DateTime chargeTime = new DateTime(driveDate.Year, driveDate.Month, driveDate.Day, driveTime.Hour, driveTime.Minute, driveTime.Second, DateTimeKind.Utc).AddHours(-hoursNeeded);

                chargeTime = TimeZoneInfo.ConvertTime(chargeTime, _copenhagenTimeZoneInfo);

                var response = new DialogFlowResponseDTO();
                response.outputContexts = request.queryResult.outputContexts;

                if (doCharge)
                {
                    // do something to remember to charge the car
                }
                response.fulfillmentText = request.queryResult.fulfillmentText
                                .Replace("$chargeDay", chargeTime.DayOfWeek.ToString())
                                .Replace("$chargeTime", chargeTime.ToShortTimeString());

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
