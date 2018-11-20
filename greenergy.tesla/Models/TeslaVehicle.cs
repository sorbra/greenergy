using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Greenergy.Tesla
{
    public class TeslaVehicle
    {
        public TeslaOwner Owner;
        public string Id { get; set; }
        public string VIN { get; set; }
        public string DisplayName { get; set; }
        public bool InService;
        public string State;

        public async Task<TeslaChargeState> GetChargeStateAsync()
        {
            var response = await Owner.Client.GetAsync(TeslaAPI.CHARGESTATE_PATH.Replace("{id}", Id));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to get Tesla ChargeState. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var cs = JsonConvert.DeserializeObject<TeslaChargingStateResponseDTO>(
                await response.Content.ReadAsStringAsync()
            );

            var chargeState = new TeslaChargeState()
            {
                ChargingState = cs.ChargeStateDTO.ChargingState,
                BatteryLevel = cs.ChargeStateDTO.BatteryLevel,
                TimeToFullCharge = cs.ChargeStateDTO.TimeToFullCharge,
                ScheduledChargingPending = cs.ChargeStateDTO.ScheduledChargingPending
            };

            return chargeState;
        }

        protected async Task<ResponseDTOType> PostCommand<ResponseDTOType>(string command, object requestBody)
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

            var responseDTO = JsonConvert.DeserializeObject<ResponseDTOType>(
                await response.Content.ReadAsStringAsync()
            );

            return responseDTO;
        }

        public async Task<bool> StartCharge()
        {
            var response = await PostCommand<TeslaCommandResponseDTO>("start_charge", null);
            return response.response;
        }

    }
}