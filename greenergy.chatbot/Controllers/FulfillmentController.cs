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
using System.Globalization;

namespace greenergy.chatbot_fulfillment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private const float kilometersPerKwh = 5f;
        private const float kwhPerHour = 10f;

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
//                var intent = request.queryResult.intent.name;
                var action = request.queryResult.action;
                if (action.Equals("chargecar.intent.yes"))
                {
                    return await HandleChargeCarIntent(request, true);
                }
                else if (action.Equals("chargecar.intent.no"))
                {
                    return await HandleChargeCarIntent(request, false);
                }
                else if (action.Equals("consume.electricity"))
                {
                    return await HandleConsumeElectricityIntent(request);
                }
                else if (action.Equals("send.charging.reminder.yes"))
                {
                    return await HandleSendReminderIntent(request);
                }
                else if (action.Equals("request.currentemissions"))
                {
                    return await HandleCurrentCo2QueryIntent(request);
                }
                else if (action.Equals("consume.electricity.explain"))
                {
                    return await HandleConsumeElectricityExplain(request);
                }
                else
                {
                    _logger.LogError($"FulfillmentController.Fulfill: Unknown action: {action}");
                    return NotFound();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in FulfillmentController.Fulfill", null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleConsumeElectricityExplain(DialogFlowRequestDTO request)
        {
            try
            {
                Parameters parameters = request.queryResult.outputContexts
                            .FirstOrDefault(oc => oc.name.EndsWith("consumeelectricity-followup"))
                            .parameters;

                DialogFlowResponseDTO response = new DialogFlowResponseDTO();
                response.outputContexts = request.queryResult.outputContexts;
                var co2perkwh = parameters.currentemissions;

                response.fulfillmentText = request.queryResult.fulfillmentText
                            .Replace("$currentemissions", parameters.currentemissions.ToString())
                            .Replace("$optimalemissions", parameters.optimalemissions.ToString())
                            .Replace("$savingspercentage", parameters.savingspercentage.ToString());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                            .FirstOrDefault(oc => oc.name.EndsWith("consume-electricity-output-context"))
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
                    response.outputContexts = request.queryResult.outputContexts;

                    var optimalConsumptionStart = TimeZoneInfo.ConvertTime(best.optimalConsumptionStart, _copenhagenTimeZoneInfo);
                    var prognosisEnd = TimeZoneInfo.ConvertTime(best.lastPrognosisTime, _copenhagenTimeZoneInfo).AddMinutes(5);
                    var now = TimeZoneInfo.ConvertTime(DateTime.Now, _copenhagenTimeZoneInfo);
                    var prognosisLookaheadHours = Math.Round((prognosisEnd - now).TotalHours, 0);

                    var lang = request.queryResult.languageCode;
                    var culture = CultureInfo.CreateSpecificCulture(lang);

                    float savingsPercentage = (best.currentEmissions - best.optimalEmissions) / best.currentEmissions;

                    OutputContext ctx = response.outputContexts
                                        .Find(oc => oc.name.EndsWith("consumeelectricity-followup"));
                    ctx.parameters.prognosisend = prognosisEnd;
                    ctx.parameters.savingspercentage = (float)Math.Round(savingsPercentage * 100, 0);
                    ctx.parameters.optimalemissions = best.optimalEmissions;
                    ctx.parameters.currentemissions = best.currentEmissions;
                    ctx.parameters.optimalconsumptionstart = optimalConsumptionStart.ToString("dddd HH:mm", culture);

                    response.fulfillmentText = request.queryResult.fulfillmentText
                                .Replace("$optimalEmissions", best.optimalEmissions.ToString())
                                .Replace("$consumption-start", optimalConsumptionStart.ToString("dddd HH:mm", culture))
                                .Replace("$prognosis-end", prognosisEnd.ToString("dddd HH:mm", culture))
                                .Replace("$savingspercentage", ctx.parameters.savingspercentage.ToString())
                                .Replace("$prognosislookaheadhours", prognosisLookaheadHours.ToString() + " hours");
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
                                        .FirstOrDefault(oc => oc.name.EndsWith("drivesomewhereintent-followup"))
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
