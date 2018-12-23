using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;
using Microsoft.Extensions.Logging;
using Greenergy.Emissions.Messaging;

namespace Greenergy.Emissions.Optimization
{
    class ConsumptionOptimizer : IConsumptionOptimizer
    {
        private ILogger<ConsumptionOptimizer> _logger;

        public ConsumptionOptimizer(ILogger<ConsumptionOptimizer> logger)
        {
            _logger = logger;
        }

        // Assumption: prognoses is ordered by EmissionTimeUTC ascending
        public OptimalConsumptionPrognosis SuggestConsumption(string region, int hours, List<EmissionDataDTO> prognoses)
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

            return new OptimalConsumptionPrognosis()
            {
                Region = region,
                HoursOfConsumption = hours,
                Earliest = new PredictedConsumption
                {
                    StartUTC = prognoses.First().EmissionTimeUTC,
                    Emissions = initialEmissions / windowSize
                },
                Best = new PredictedConsumption
                {
                    StartUTC = prognoses[inxMinStart].EmissionTimeUTC,
                    Emissions = minTotalEmissions / windowSize
                },
                Latest = new PredictedConsumption
                {
                    StartUTC = prognoses[inxStart].EmissionTimeUTC,
                    Emissions = curTotalEmissions / windowSize
                },
                PrognosisTimeUTC = prognoses.First().RecordedTimeUTC,
                PrognosisEndUTC = prognoses.Last().EmissionTimeUTC
            };
        }

        private List<OptimalConsumptionPrognosis> ExtractRegionalConsumptionInfo(List<EmissionDataDTO> prognoses, string region)
        {
            var prognosisHours = (int)
                (prognoses.Last().EmissionTimeUTC - prognoses.First().EmissionTimeUTC)
                .TotalHours;

            var regionalConsumptionInfo = new List<OptimalConsumptionPrognosis>();
            if (prognosisHours < 1)
            {
                // not enough data
                return regionalConsumptionInfo;
            }

            for (var hours = 1; hours <= prognosisHours; hours++)
            {
                regionalConsumptionInfo.Add(SuggestConsumption(region, hours, prognoses));
            }
            return regionalConsumptionInfo;
        }

        public List<RegionalConsumptionPrognoses> SuggestConsumption(List<EmissionDataDTO> prognoses)
        {
            // Split into regions
            var prognosisRegions = prognoses.GroupBy(p => p.Region);

            var regionalInfos = new List<RegionalConsumptionPrognoses>();
            foreach (var r in prognosisRegions)
            {
                var rPrognoses = r.OrderBy(p => p.EmissionTimeUTC).ToList();
                regionalInfos.Add( new RegionalConsumptionPrognoses() {
                                            Region = r.Key,
                                            Prognoses = ExtractRegionalConsumptionInfo(rPrognoses, r.Key)
                });
            }

            return regionalInfos;
        }
    }
}