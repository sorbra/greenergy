using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using greenergy.chatbot_fulfillment.RequestModels;
using greenergy.chatbot_fulfillment.ResponseModels;

namespace greenergy.chatbot_fulfillment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private const float kilometersPerKwh = 5f;
        private const float kwhPerHour = 10f;

        private const string _handleChargeCarIntent = "projects/greenergy-3dbfe/agent/intents/5845fca2-2532-4aca-ab69-d89947557032";
        private const string _handleCurrentCo2QueryIntent = "projects/greenergy-3dbfe/agent/intents/6e9a8963-9a74-4343-8e0d-a7b2563db55c";

        // POST api/values
        [HttpPost]
        public ActionResult<DialogFlowResponseDTO> Post([FromBody] DialogFlowRequestDTO request)
        {
            var intent = request.queryResult.intent.name;
            if (intent.Equals(_handleChargeCarIntent))
            {
                return HandleChargeCarIntent(request);
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

            
            DialogFlowResponseDTO response = new DialogFlowResponseDTO();
            response.fulfillmentText = "I honestly have no idea. But, in a few days I should know more. Please come back later!!";
            return response;
        }

        private ActionResult<DialogFlowResponseDTO> HandleChargeCarIntent(DialogFlowRequestDTO request)
        {
            DateTime driveTime = request.queryResult.parameters.Time;
            DateTime driveDate = request.queryResult.parameters.Date;

            float kilometers = request.queryResult.parameters.Kilometers;
            float hoursNeeded = kilometers / kilometersPerKwh / kwhPerHour;

            Boolean doCharge = request.queryResult.parameters.doCharge.Equals("yes");

            // Retrieve the time zone for Copenhagen Denmark (Romance Standard Time).
            TimeZoneInfo cet;
            try
            {
                cet = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                driveTime = TimeZoneInfo.ConvertTime(driveTime, cet);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve the Romance Standard Time zone.");
            }

            DateTime chargeTime = new DateTime(driveDate.Year, driveDate.Month, driveDate.Day, driveTime.Hour, driveTime.Minute, driveTime.Second).AddHours(-hoursNeeded);

            DialogFlowResponseDTO response = new DialogFlowResponseDTO();
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
