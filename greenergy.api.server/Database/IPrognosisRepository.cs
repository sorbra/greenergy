using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Models;

namespace Greenergy.Database
{
    public interface IPrognosisRepository
    {
        Task UpdatePrognosisData(List<PrognosisData> prognoses);

        Task<ConsumptionInfo> OptimalFutureConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime finishNoLaterThan);
    };

    public class ConsumptionInfo
    {
        public int co2perkwh { get; set; }
        public DateTime consumptionStart { get; set; }
        public int consumptionMinutes { get; set; }
        public string consumptionRegion { get; set; }
        public DateTime lastPrognosisUpdateTime { get; set; }
    }
}