using System;

namespace Greenergy.Emissions.API.Client.Models
{
    public class PredictedConsumption
    {
        public DateTimeOffset StartUTC { get; set; }
        public float Emissions { get; set; }
    }

    public class OptimalConsumptionPrognosis
    {
        public string Region { get; set; }
        public int HoursOfConsumption { get; internal set; }
        public DateTimeOffset PrognosisEndUTC { get; set; }
        public PredictedConsumption Earliest { get; set; }
        public PredictedConsumption Latest { get; set; }
        public PredictedConsumption Best { get; set; }
        public DateTimeOffset PrognosisTimeUTC { get; set; }
    }
}