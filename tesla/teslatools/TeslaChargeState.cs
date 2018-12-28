namespace Greenergy.TeslaTools
{
    public class TeslaChargeState
    {
        public string ChargingState;
        public int BatteryLevel;
        public decimal TimeToFullCharge;
        public bool ScheduledChargingPending;
        public int ChargeLimit;
    }
}