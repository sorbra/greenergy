using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using greenergy.chatbot_fulfillment.Models;
using Greenergy.API;
using Microsoft.Extensions.Logging;

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

        private IGreenergyAPIClient _greenergyAPIClient;
        private ILogger<FulfillmentController> _logger;

        public FulfillmentController(IGreenergyAPIClient greenergyAPIClient, ILogger<FulfillmentController> logger)
        {
            _greenergyAPIClient = greenergyAPIClient;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<EmissionDataDTO>> Getemissions()
        {
            var emissions = _greenergyAPIClient.GetLatest().Result;
            return emissions;
        }

        // POST api/values
        [HttpPost]
        public ActionResult<DialogFlowResponseDTO> Post([FromBody] DialogFlowRequestDTO request)
        {
            var intent = request.queryResult.intent.name;
            if (intent.Equals(_handleChargeCarYESIntent))
            {
                var response = HandleChargeCarIntent(request, true);

                _logger.LogInformation(response.ToString());

                return response;
            }
            else if (intent.Equals(_handleChargeCarNOIntent))
            {
                return HandleChargeCarIntent(request, false);
            }
            else if (intent.Equals(_handleCurrentCo2QueryIntent))
            {
                return HandleCurrentCo2QueryIntent(request);
            }
            else
            {
                return NotFound();
            }
        }

        private ActionResult<DialogFlowResponseDTO> HandleCurrentCo2QueryIntent(DialogFlowRequestDTO request)
        {
            var emissions = _greenergyAPIClient.GetLatest().Result;

            var currentEmission = emissions[0].Emission;

            DialogFlowResponseDTO response = new DialogFlowResponseDTO();
            response.fulfillmentText = $"Current co2 emission is {currentEmission} grams co2 per kilowatt hour";

            return response;
        }

        private ActionResult<DialogFlowResponseDTO> HandleChargeCarIntent(DialogFlowRequestDTO request, Boolean doCharge)
        {
            var driveSomewhereContext = request.queryResult.outputContexts[0];
            var parameters = driveSomewhereContext.parameters;

            DateTime driveTime = parameters.time.ToUniversalTime();
            DateTime driveDate = parameters.time.Date;

            float hoursNeeded = parameters.kilometers / kilometersPerKwh / kwhPerHour;

            _logger.LogDebug($"HandleChargeCarIntent received parameters. Date={driveDate}, Time={driveTime.ToUniversalTime()}, Kilometers={parameters.kilometers}");

            // Retrieve the time zone for Copenhagen Denmark (Romance Standard Time).
            TimeZoneInfo cet;
            try
            {
                cet = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                driveTime = TimeZoneInfo.ConvertTime(driveTime, cet);
            }
            catch (Exception)
            {
                _logger.LogCritical("HandleChargeCarIntent: Unable to retrieve the Romance Standard Time zone.");
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
