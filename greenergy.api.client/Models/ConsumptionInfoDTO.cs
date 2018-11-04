using System;

namespace Greenergy.API.Models
{
    public class ConsumptionInfoDTO
    {
        public int optimalCo2perkwh { get; set; }
        public int currentCo2perkwh { get; set; }
        public DateTime consumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime lastPrognosisUpdateTime { get; set; }
    }
}