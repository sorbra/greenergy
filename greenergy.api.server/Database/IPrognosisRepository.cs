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
        public int co2perkwh { get; set; }
        public DateTime consumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime lastPrognosisUpdateTime { get; set; }

        public static explicit operator ConsumptionInfoDTO(ConsumptionInfoMongo cim)
        {
            return new ConsumptionInfoDTO
            {
                optimalCo2perkwh = cim.co2perkwh,
                consumptionStart = cim.consumptionStart,
                consumptionMinutes = cim.consumptionMinutes,
                consumptionRegion = cim.consumptionRegion,
                lastPrognosisUpdateTime = cim.lastPrognosisUpdateTime
            };
        }
    }
}   