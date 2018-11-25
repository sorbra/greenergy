using System;

namespace Greenergy.Emissions.API.Client.Models
{
    public class ConsumptionInfoDTO
    {
        public float FirstEmissions { get; set; }
        public float OptimalEmissions { get; set; }
        public float LastEmissions { get; set; }
        public DateTimeOffset FirstConsumptionStartUTC { get; set; }
        public DateTimeOffset OptimalConsumptionStartUTC { get; set; }
        public DateTimeOffset LastConsumptionStartUTC { get; set; }
        public int ConsumptionMinutes { get; set; }
        public string ConsumptionRegion { get; set; }
        public DateTimeOffset PrognosisUpdateTimeUTC { get; set; }
        public DateTimeOffset LastPrognosisTimeUTC { get; set; }
        public int ConsumptionHours { get; internal set; }
    }
}