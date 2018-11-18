using System;
using System.Net;
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
            var response = await Owner.Client.GetAsync(TeslaAPI.GET_CHARGESTATE_PATH.Replace("{id}",Id));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to get Tesla ChargeState. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var cs = JsonConvert.DeserializeObject<TeslaChargingStateResponseDTO>(
                await response.Content.ReadAsStringAsync()
            );

            var chargeState = new TeslaChargeState() {
                ChargingState = cs.ChargeStateDTO.ChargingState,
                BatteryLevel = cs.ChargeStateDTO.BatteryLevel,
                TimeToFullCharge = cs.ChargeStateDTO.TimeToFullCharge,
                ScheduledChargingPending = cs.ChargeStateDTO.ScheduledChargingPending
            };

            return chargeState;
        }

    }
}