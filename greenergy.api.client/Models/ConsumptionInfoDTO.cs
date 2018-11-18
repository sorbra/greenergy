using System;

namespace Greenergy.API.Models
{
    public class ConsumptionInfoDTO
    {
        public float firstEmissions { get; set; }
        public float optimalEmissions { get; set; }
        public float lastEmissions { get; set; }
        public DateTime firstConsumptionStartUTC { get; set; }
        public DateTime optimalConsumptionStartUTC { get; set; }
        public DateTime lastConsumptionStartUTC { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime prognosisUpdateTimeUTC { get; set; }
        public DateTime lastPrognosisTimeUTC { get; set; }
    }
}