using System;
using Newtonsoft.Json;

namespace Greenergy.Tesla
{
    [JsonObject]
    class TeslaChargingStateResponseDTO
    {
        [JsonProperty("response")]
        public TeslaChargeStateDTO ChargeStateDTO;
    }

    [JsonObject]
    class TeslaChargeStateDTO
    {
        [JsonProperty("charging_state")]
        public string ChargingState;
        [JsonProperty("battery_level")]
        public int BatteryLevel;
        [JsonProperty("time_to_full_charge")]
        public decimal TimeToFullCharge;
        [JsonProperty("scheduled_charging_pending")]
        public bool ScheduledChargingPending;
    }
}