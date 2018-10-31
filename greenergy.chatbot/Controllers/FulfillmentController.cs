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
                _logger.LogError(ex, "Exception in FulfillmentController.Getemissions", null);
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
                else
                {
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
            var emissions = await _greenergyAPI.GetMostRecentEmissions();

            var currentEmission = emissions[0].Emission;

            DialogFlowResponseDTO response = new DialogFlowResponseDTO();
            response.fulfillmentText = $"Current co2 emission is {currentEmission} grams co2 per kilowatt hour";

            return response;
        }

        private async Task<ActionResult<DialogFlowResponseDTO>> HandleChargeCarIntent(DialogFlowRequestDTO request, Boolean doCharge)
        {
            var driveSomewhereContext = request.queryResult.outputContexts[0];
            var parameters = driveSomewhereContext.parameters;

            DateTime driveTime = parameters.time.ToUniversalTime();
            DateTime driveDate = parameters.time.Date;

            float hoursNeeded = parameters.kilometers / kilometersPerKwh / kwhPerHour;

            // Retrieve the time zone for Copenhagen Denmark (Romance Standard Time).
            TimeZoneInfo cet;
            String tzName = _config.Value.TimeZone; // "Romance Standard Time"; // "Europe/Copenhagen"; 
            try
            {
                cet = TimeZoneInfo.FindSystemTimeZoneById(tzName);
                driveTime = TimeZoneInfo.ConvertTime(driveTime, cet);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"HandleChargeCarIntent: Unable to retrieve the {tzName} Time zone.");
                throw ex;
            }

            DateTime chargeTime = new DateTime(driveDate.Year, driveDate.Month, driveDate.Day, driveTime.Hour, driveTime.Minute, driveTime.Second).AddHours(-hoursNeeded);

            var response = new DialogFlowResponseDTO();
            response.outputContexts = request.queryResult.outputContexts;

            if (doCharge)
            {
                response.fulfillmentText = $"I will charge your car. Please plug in your car no later than {chargeTime.DayOfWeek} at {chargeTime.ToShortTimeString()} to ensure timely charging.";
            }
            else
            {
                response.fulfillmentText = $"Start charging no later than {chargeTime.DayOfWeek} at {chargeTime.ToShortTimeString()} to ensure timely charging.";
            }

            return response;
        }
    }
}
