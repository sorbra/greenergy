using System;
using Newtonsoft.Json;

namespace Greenergy.Tesla
{
    [JsonObject]
    public class TeslaCommandResponseDTO<TeslaResponseType>
    {
        [JsonProperty("response")]
        public TeslaResponseType response;
    }

    [JsonObject]
    public class TeslaChargeStateResponseType
    {
        [JsonProperty("charging_state")]
        public string ChargingState;
        [JsonProperty("battery_level")]
        public int BatteryLevel;
        [JsonProperty("time_to_full_charge")]
        public decimal TimeToFullCharge;
        [JsonProperty("scheduled_charging_pending")]
        public bool ScheduledChargingPending;
        [JsonProperty("charge_limit_soc")]
        public int ChargeLimit;
    }
}