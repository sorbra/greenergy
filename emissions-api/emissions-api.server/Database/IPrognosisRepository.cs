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

        Task<ConsumptionInfoMongo> OptimalConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThanUTC, DateTime finishNoLaterThanUTC);
    };

    public class ConsumptionInfoMongo
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

        public static explicit operator ConsumptionInfoDTO(ConsumptionInfoMongo cim)
        {
            return new ConsumptionInfoDTO
            {
                FirstEmissions = cim.FirstEmissions,
                OptimalEmissions = cim.OptimalEmissions,
                LastEmissions = cim.LastEmissions,
                FirstConsumptionStartUTC = cim.FirstConsumptionStartUTC,
                OptimalConsumptionStartUTC = cim.OptimalConsumptionStartUTC,
                LastConsumptionStartUTC = cim.LastConsumptionStartUTC,
                ConsumptionMinutes = cim.ConsumptionMinutes,
                ConsumptionRegion = cim.ConsumptionRegion,
                PrognosisUpdateTimeUTC = cim.PrognosisUpdateTimeUTC,
                LastPrognosisTimeUTC = cim.LastPrognosisTimeUTC
            };
        }
    }
}   