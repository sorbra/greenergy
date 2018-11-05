using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.API.Models;
using Greenergy.Models;

namespace Greenergy.Database
{
    public interface IPrognosisRepository
    {
        Task UpdatePrognosisData(List<PrognosisDataMongo> prognoses);

        Task<ConsumptionInfoMongo> OptimalFutureConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThan, DateTime finishNoLaterThan);
    };

    public class ConsumptionInfoMongo
    {
        public float optimalEmissions { get; set; }
        public float currentEmissions { get; set; }
        public DateTime optimalConsumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime prognosisUpdateTime { get; set; }
        public DateTime lastPrognosisTime { get; set; }

        public static explicit operator ConsumptionInfoDTO(ConsumptionInfoMongo cim)
        {
            return new ConsumptionInfoDTO
            {
                currentEmissions = cim.currentEmissions,
                optimalEmissions = cim.optimalEmissions,
                optimalConsumptionStart = cim.optimalConsumptionStart,
                consumptionMinutes = cim.consumptionMinutes,
                consumptionRegion = cim.consumptionRegion,
                prognosisUpdateTime = cim.prognosisUpdateTime,
                lastPrognosisTime = cim.lastPrognosisTime
            };
        }
    }
}   