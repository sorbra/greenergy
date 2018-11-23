using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Server.Models.Mongo;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Database
{
    public interface IPrognosisRepository
    {
        Task UpdatePrognosisData(List<PrognosisDataMongo> prognoses);

        Task<ConsumptionInfoMongo> OptimalConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThan, DateTime finishNoLaterThan);
    };

    public class ConsumptionInfoMongo
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

        public static explicit operator ConsumptionInfoDTO(ConsumptionInfoMongo cim)
        {
            return new ConsumptionInfoDTO
            {
                firstEmissions = cim.firstEmissions,
                optimalEmissions = cim.optimalEmissions,
                lastEmissions = cim.lastEmissions,
                firstConsumptionStartUTC = cim.firstConsumptionStartUTC,
                optimalConsumptionStartUTC = cim.optimalConsumptionStartUTC,
                lastConsumptionStartUTC = cim.lastConsumptionStartUTC,
                consumptionMinutes = cim.consumptionMinutes,
                consumptionRegion = cim.consumptionRegion,
                prognosisUpdateTimeUTC = cim.prognosisUpdateTimeUTC,
                lastPrognosisTimeUTC = cim.lastPrognosisTimeUTC
            };
        }
    }
}   