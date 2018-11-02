using System;

namespace Greenergy.API.Models
{
    public class ConsumptionInfoDTO
    {
        public int co2perkwh { get; set; }
        public DateTime consumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime lastPrognosisUpdateTime { get; set; }
    }
}