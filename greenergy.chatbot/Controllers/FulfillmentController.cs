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

namespace greenergy.chatbot_fulfillment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private const float kilometersPerKwh = 5f;
        private const float kwhPerHour = 10f;

        private const string _handleChargeCarYESIntent = "projects/greenergy-3dbfe/agent/intents/07a5c59b-1bdc-4dc5-8788-5380b48c5324";
        private const string _handleChargeCarNOIntent = "projects/greenergy-3dbfe/agent/intents/63256d8b-9a44-4dd3-829f-47dc8073bc8c";
        private const string _handleCurrentCo2QueryIntent = "projects/greenergy-3dbfe/agent/intents/6e9a8963-9a74-4343-8e0d-a7b2563db55c";
        private const string _handleSendReminderIntent = "projects/greenergy-3dbfe/agent/intents/2f19e6a9-a5c9-404c-a4e0-4d34f90edeba";

        private const string _drivesomewhereintentFollowupContext = "drivesomewhereintent-followup";
        private const string _consumeElectricityOutputContext = "consume-electricity-output-context";

        private IGreenergyAPI _greenergyAPI;
        private ILogger<FulfillmentController> _logger;
        private IOptions<FulfillmentSettings> _config;

        public FulfillmentController(IGreenergyAPI greenergyAPI, ILogger<FulfillmentController> logger, IOptions<FulfillmentSettings> config)
        {
            _greenergyAPI = greenergyAPI;
            _logger = logger;
            _config = config;
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
                _logger.LogError(ex, "FulfillmentController.Getemissions", null);
                return null;
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<DialogFlowResponseDTO>> Fulfill([FromBody] DialogFlowRequestDTO request)
        {
            try
            {
                var intent = request.queryResult.intent.name;
                if (intent.Equals(_handleChargeCarYESIntent))
                {
                    return await HandleChargeCarIntent(request, true);
                }
                else if (intent.Equals(_handleChargeCarNOIntent))
                {
                    return await HandleChargeCarIntent(request, false);
                }
                else if (intent.Equals(_handleCurrentCo2QueryIntent))
                {
                    return await HandleCurrentCo2QueryIntent(request);
                }
                else if (intent.Equals(_handleSendReminderIntent))
                {
                    return await HandleSendReminderIntent(request);
                }
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

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleSendReminderIntent(DialogFlowRequestDTO request)
        {
            var response = new DialogFlowResponseDTO();
            response.outputContexts = request.queryResult.outputContexts;

            response.fulfillmentText = request.queryResult.fulfillmentText;

            return response;
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleCurrentCo2QueryIntent(DialogFlowRequestDTO request)
        {

            Parameters parameters = request.queryResult.outputContexts
                        .FirstOrDefault(oc => oc.name.EndsWith(_consumeElectricityOutputContext))
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

                response.fulfillmentText = request.queryResult.fulfillmentText
                            .Replace("$co2perkwh", best.co2perkwh.ToString())
                            .Replace("$optimal-time", best.consumptionStart.ToShortTimeString())
                            .Replace("$optimal-day", best.consumptionStart.DayOfWeek.ToString());

                return response;
            }
            return null;
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleChargeCarIntent(DialogFlowRequestDTO request, Boolean doCharge)
        {
            var parameters = request.queryResult.outputContexts
                                        .FirstOrDefault(oc => oc.name.EndsWith(_drivesomewhereintentFollowupContext))
                                        .parameters;

            DateTime driveTime = parameters.time.ToUniversalTime();
            DateTime driveDate = parameters.time.Date;

            float hoursNeeded = parameters.kilometers / kilometersPerKwh / kwhPerHour;

            DateTime chargeTime = new DateTime(driveDate.Year, driveDate.Month, driveDate.Day, driveTime.Hour, driveTime.Minute, driveTime.Second, DateTimeKind.Utc).AddHours(-hoursNeeded);

            try
            {
                var cet = TimeZoneInfo.FindSystemTimeZoneById(_config.Value.TimeZone);
                chargeTime = TimeZoneInfo.ConvertTime(chargeTime, cet);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                throw (ex);
            }

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
    }
}
