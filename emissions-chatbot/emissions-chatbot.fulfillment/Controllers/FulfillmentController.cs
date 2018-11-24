﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using greenergy.chatbot_fulfillment.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Greenergy.Emissions.API;
using System.Net.Http;

namespace greenergy.chatbot_fulfillment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private const float kilometersPerKwh = 5f;
        private const float kwhPerHour = 10f;

        private ILogger<FulfillmentController> _logger;
        private IOptions<FulfillmentSettings> _config;
        private HttpClient _httpClient;
        private TimeZoneInfo _copenhagenTimeZoneInfo;
        private EmissionsClient EmissionsClient
        {
            get
            {
                var client = new EmissionsClient(_httpClient);
                client.BaseUrl = _config.Value.EmissionsServiceBaseURL;
                return client;
            }
        }
        private PrognosisClient PrognosisClient
        {
            get
            {
                var client = new PrognosisClient(_httpClient);
                client.BaseUrl = _config.Value.EmissionsServiceBaseURL;
                return client;
            }
        }
        public FulfillmentController(ILogger<FulfillmentController> logger, IOptions<FulfillmentSettings> config)
        {
            _logger = logger;
            _config = config;
            _httpClient = new HttpClient();

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
        public async Task<ActionResult<ICollection<EmissionDataDTO>>> Getemissions()
        {
            try
            {
                return new ActionResult<ICollection<EmissionDataDTO>>(await EmissionsClient.GetMostRecentEmissionsAsync());
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
                // else if (action.Equals("consume.electricity.explain"))
                // {
                //     return await HandleConsumeElectricityExplain(request);
                // }
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

        // private async Task<ActionResult<DialogFlowResponseDTO>> HandleConsumeElectricityExplain(DialogFlowRequestDTO request)
        // {
        //     try
        //     {
        //         Parameters parameters = request.queryResult.outputContexts
        //                     .FirstOrDefault(oc => oc.name.EndsWith("consumeelectricity-followup"))
        //                     .parameters;

        //         DialogFlowResponseDTO response = new DialogFlowResponseDTO();
        //         response.outputContexts = request.queryResult.outputContexts;
        //         var co2perkwh = parameters.initialemissions;

        //         response.fulfillmentText = request.queryResult.fulfillmentText
        //                     .Replace("$initialemissions", parameters.initialemissions.ToString())
        //                     .Replace("$optimalemissions", parameters.optimalemissions.ToString())
        //                     .Replace("$savingspercentage", parameters.savingspercentage.ToString());
        //         return response;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, ex.Message, null);
        //         return StatusCode(StatusCodes.Status500InternalServerError);
        //     }
        // }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleCurrentCo2QueryIntent(DialogFlowRequestDTO request)
        {
            try
            {
                var currentEmission = (await EmissionsClient.GetMostRecentEmissionsAsync()).FirstOrDefault().Emission;

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
                var response = new DialogFlowResponseDTO() 
                {
                    outputContexts = request.queryResult.outputContexts,
                    fulfillmentText = request.queryResult.fulfillmentText
                };
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

                var nowUTC = DateTime.Now.ToUniversalTime();
                var now = TimeZoneInfo.ConvertTime(nowUTC, _copenhagenTimeZoneInfo);

                DateTime finishNoLaterThanUTC;

                if (parameters.time != DateTime.MinValue && parameters.date != DateTime.MinValue)
                {
                    DateTime byTime = parameters.time.ToUniversalTime();
                    DateTime byDate = parameters.time.Date;
                    finishNoLaterThanUTC = new DateTime(byDate.Year, byDate.Month, byDate.Day, byTime.Hour, byTime.Minute, byTime.Second, DateTimeKind.Utc);
                }
                else
                {
                    finishNoLaterThanUTC = now.AddDays(1).Date.AddHours(6).ToUniversalTime();
                    if ((finishNoLaterThanUTC - nowUTC).TotalHours < 6) finishNoLaterThanUTC = finishNoLaterThanUTC.AddDays(1);
                }

                if (parameters.duration == null)
                {
                    if (parameters.devicetype != null)
                    {
                        parameters.duration = LookupDefaultDeviceDuration(parameters.devicetype);
                    }
                    else
                    {
                        parameters.duration = new Duration { amount = 1, unit = "h" };
                    }
                }

                var best = await PrognosisClient.OptimalConsumptionTimeAsync(
                    consumptionMinutes: parameters.duration.toMinutes(),
                    consumptionRegion: "DK1",
                    startNoEarlierThan: nowUTC,
                    finishNoLaterThan: finishNoLaterThanUTC
                );

                if (best != null)
                {
                    DialogFlowResponseDTO response = new DialogFlowResponseDTO();
                    response.outputContexts = request.queryResult.outputContexts;

                    var optimalConsumptionStart = TimeZoneInfo.ConvertTime(best.OptimalConsumptionStartUTC, _copenhagenTimeZoneInfo);
                    var prognosisEnd = TimeZoneInfo.ConvertTime(best.LastPrognosisTimeUTC, _copenhagenTimeZoneInfo).AddMinutes(5);
                    var prognosisLookaheadHours = Math.Round((best.LastPrognosisTimeUTC - nowUTC).TotalHours, 0);
                    var finishNoLaterThan = TimeZoneInfo.ConvertTime(finishNoLaterThanUTC, _copenhagenTimeZoneInfo);

                    var lang = request.queryResult.languageCode;
                    var culture = CultureInfo.CreateSpecificCulture(lang);

                    double savingsPercentage = (best.FirstEmissions - best.OptimalEmissions) / best.FirstEmissions;

                    OutputContext ctx = response.outputContexts
                                        .Find(oc => oc.name.EndsWith("consumeelectricity-followup"));
                    ctx.parameters.prognosisend = best.LastPrognosisTimeUTC;
                    ctx.parameters.savingspercentage = (float)Math.Round(savingsPercentage * 100, 0);
                    ctx.parameters.optimalemissions = Math.Round(best.OptimalEmissions, 0);
                    ctx.parameters.initialemissions = Math.Round(best.FirstEmissions, 0);
                    ctx.parameters.lastEmissions = Math.Round(best.LastEmissions, 0);
                    ctx.parameters.optimalconsumptionstart = optimalConsumptionStart.ToString("dddd HH:mm", culture);
                    ctx.parameters.finishnolaterthan = finishNoLaterThan.ToString("dddd HH:mm", culture);
                    ctx.parameters.readableduration = parameters.duration.toReadableString();
                    ctx.parameters.waitinghours = Math.Round((best.OptimalConsumptionStartUTC - nowUTC).TotalHours, 0);

                    // ctx.parameters.test1 = optimalConsumptionStart.ToUniversalTime().ToString("o");
                    // ctx.parameters.test2 = best.optimalConsumptionStart.ToUniversalTime().ToString("o");

                    if (ctx.parameters.savingspercentage < 2)
                    {
                        response.fulfillmentText = "Now is a good time! Do you want to know why?"
                                    .Replace("$device", ctx.parameters.devicetype)
                                    .Replace("$duration", parameters.duration.toReadableString());
                    }
                    else
                    {
                        response.fulfillmentText = request.queryResult.fulfillmentText
                                    .Replace("$optimalEmissions", ctx.parameters.optimalemissions.ToString())
                                    .Replace("$consumption-start", ctx.parameters.optimalconsumptionstart)
                                    .Replace("$prognosis-end", prognosisEnd.ToString("dddd HH:mm", culture))
                                    .Replace("$savingspercentage", ctx.parameters.savingspercentage.ToString())
                                    .Replace("$prognosislookaheadhours", prognosisLookaheadHours.ToString() + " hours")
                                    .Replace("$finishnolaterthan", ctx.parameters.finishnolaterthan)
                                    .Replace("$readableduration", ctx.parameters.readableduration)
                                    .Replace("$waitinghours", ctx.parameters.waitinghours.ToString());
                    }

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

        private Duration LookupDefaultDeviceDuration(string devicetype)
        {
            var amount = 1;
            if (devicetype.ToLower().Equals("dishwasher"))
            {
                amount = 2;
            }
            else if (devicetype.ToLower().Equals("tumbledryer"))
            {
                amount = 2;
            }
            else if (devicetype.ToLower().Equals("washing machine"))
            {
                amount = 3;
            }
            else if (devicetype.ToLower().Equals("car"))
            {
                amount = 2;
            }
            return new Duration { amount = amount, unit = "h" };
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

                double hoursNeeded = parameters.kilometers / kilometersPerKwh / kwhPerHour;

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
