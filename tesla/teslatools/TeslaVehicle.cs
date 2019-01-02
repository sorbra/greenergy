using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Greenergy.Tesla.API;

namespace Greenergy.TeslaTools
{
    public class TeslaVehicle
    {
        public TeslaOwner Owner;
        public string Id { get; set; }
        public string VIN { get; set; }
        public string DisplayName { get; set; }
        // public bool InService;
        // public string State;

        public async Task<TeslaChargeState> GetChargeStateAsync()
        {
            var response = await Owner.Client.GetAsync(TeslaAPI.CHARGESTATE_PATH.Replace("{id}", Id));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to get Tesla ChargeState. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var responseStr = await response.Content.ReadAsStringAsync();

            var cs = JsonConvert.DeserializeObject<TeslaCommandResponseDTO<TeslaChargeStateResponseType>>(
                responseStr
            );

            var chargeState = new TeslaChargeState()
            {
                ChargingState = cs.response.ChargingState,
                BatteryLevel = cs.response.BatteryLevel,
                TimeToFullCharge = cs.response.TimeToFullCharge,
                ScheduledChargingPending = cs.response.ScheduledChargingPending,
                ChargeLimit = cs.response.ChargeLimit
            };

            return chargeState;
        }

        protected async Task<TeslaCommandResponseDTO<ResponseType>> PostCommand<ResponseType>(string command, object requestBody)
        {
            string url = TeslaAPI.COMMAND_PATH
                        .Replace("{id}", Id)
                        .Replace("{command}", command);

            var request = JsonConvert.SerializeObject(requestBody);

            var response = await Owner.Client.PostAsync(
                url,
                new StringContent(
                        request,
                        Encoding.UTF8,
                        "application/json"
            ));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Command {command} failed. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var responseStr = await response.Content.ReadAsStringAsync();

            var responseDTO = JsonConvert.DeserializeObject<TeslaCommandResponseDTO<ResponseType>>(
                responseStr
            );

            return responseDTO;
        }
        public async Task<bool> StartCharge()
        {
            var response = await PostCommand<TeslaCommandResponseType>("charge_start", null);
            return response.response.result;
        }
        public async Task<bool> StopCharge()
        {
            var response = await PostCommand<TeslaCommandResponseType>("charge_stop", null);
            return response.response.result;
        }
        public async Task<bool> SetChargeLimit(int percent)
        {
            var response = await PostCommand<TeslaCommandResponseType>(
                "set_charge_limit", 
                new { percent = percent }
            );
            return response.response.result;
        }
    }
}