using System;

namespace Greenergy.API.Models
{
    public class ConsumptionInfoDTO
    {
        public float optimalEmissions { get; set; }
        public float currentEmissions { get; set; }
        public DateTime optimalConsumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime prognosisUpdateTime { get; set; }
        public DateTime lastPrognosisTime { get; set; }
    }
}