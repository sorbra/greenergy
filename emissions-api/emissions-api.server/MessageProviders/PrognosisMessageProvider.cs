using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;
using Microsoft.Extensions.Logging;

namespace Greenergy.Emissions.MessageProviders
{
    class PrognosisMessageProvider : IPrognosisMessageProvider
    {
        private ILogger<PrognosisMessageProvider> _logger;

        public PrognosisMessageProvider(ILogger<PrognosisMessageProvider> logger)
        {
            _logger = logger;
        }
        private List<ConsumptionInfoDTO> ExtractRegionalConsumptionInfo(List<EmissionDataDTO> prognoses, string region)
        {
            var prognosisHours = (int)
                (prognoses.Last().EmissionTimeUTC - prognoses.First().EmissionTimeUTC)
                .TotalHours;

            var regionalConsumptionInfo = new List<ConsumptionInfoDTO>();
            if (prognosisHours < 1)
            {
                // not enough data
                return regionalConsumptionInfo;
            }

            for (var hours = 1; hours <= prognosisHours; hours++)
            {
                int windowSize = hours * 12;
                var minTotalEmissions = prognoses.Take(windowSize).Sum(p => p.Emission);
                var initialEmissions = minTotalEmissions;
                var curTotalEmissions = minTotalEmissions;

                int inxStart = 0;
                int inxMinStart = 0;
                int inxEnd = windowSize;
                int count = prognoses.Count();

                while (inxEnd < count)
                {
                    curTotalEmissions = curTotalEmissions + prognoses[inxEnd].Emission - prognoses[inxStart].Emission;

                    inxEnd++; inxStart++;

                    if (Math.Round((double)curTotalEmissions / windowSize, 0) < Math.Round((double)minTotalEmissions / windowSize))
                    {
                        minTotalEmissions = curTotalEmissions;
                        inxMinStart = inxStart;
                    }
                }

                regionalConsumptionInfo.Add( new ConsumptionInfoDTO()
                {
                    ConsumptionHours = hours,
                    ConsumptionMinutes = hours * 60,
                    FirstEmissions = initialEmissions / windowSize,
                    OptimalEmissions = minTotalEmissions / windowSize,
                    LastEmissions = curTotalEmissions / windowSize,
                    FirstConsumptionStartUTC = prognoses.First().EmissionTimeUTC,
                    OptimalConsumptionStartUTC = prognoses[inxMinStart].EmissionTimeUTC,
                    LastConsumptionStartUTC = prognoses[inxStart].EmissionTimeUTC,
                    ConsumptionRegion = region,
                    PrognosisUpdateTimeUTC = prognoses.First().RecordedTimeUTC,
                    LastPrognosisTimeUTC = prognoses.Last().EmissionTimeUTC
                });
            }
            return regionalConsumptionInfo;
        }

        public void OnNewPrognosisData(List<EmissionDataDTO> prognoses)
        {
            var prognosisRegions = prognoses.Select(p => p.Region).Distinct();

            var regionalInfos = new List<List<ConsumptionInfoDTO>>();
            foreach (var r in prognosisRegions)
            {
                var rPrognoses = prognoses.Where(p => p.Region == r).ToList();

                var message = $"{rPrognoses.Count()} prognoses from {r}";
                _logger.LogInformation(message);

                var rci = ExtractRegionalConsumptionInfo(rPrognoses, r);
                regionalInfos.Add(rci);
            }
        }
    }
}