using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Greenergy;
using Greenergy.Emissions.API.Server.Models.Mongo;
using Greenergy.Settings;
using System.Linq;

namespace Greenergy.Database
{
    public class PrognosisRepository : IPrognosisRepository
    {
        private readonly IEmissionDataContext _context;

        public PrognosisRepository(IEmissionDataContext context)
        {
            _context = context;
        }

        public async Task UpdatePrognosisData(List<PrognosisDataMongo> prognoses)
        {
            foreach (var pg in prognoses)
            {
                var filter = Builders<PrognosisDataMongo>.Filter.Eq(pgx => pgx.Region, pg.Region)
                           & Builders<PrognosisDataMongo>.Filter.Eq(pgx => pgx.EmissionTimeUTC, pg.EmissionTimeUTC);
                var update = Builders<PrognosisDataMongo>.Update
                                .Set(pgx => pgx.Emission, pg.Emission)
                                .Set(pgx => pgx.UpdatedTimeUTC, DateTime.UtcNow);
                var options = new UpdateOptions();
                options.IsUpsert = true;
                try
                {
                    await _context.PrognosisCollection.UpdateOneAsync(filter, update, options);
                }
                catch (Exception ex)
                {
                    // log or manage the exception
                    throw ex;
                }
            }
        }

        public async Task<ConsumptionInfoMongo> OptimalConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThanUTC, DateTime finishNoLaterThanUTC)
        {
            if (startNoEarlierThanUTC.Equals(DateTime.MinValue))
            {
                startNoEarlierThanUTC = DateTime.UtcNow;
            }
            else
            {
                startNoEarlierThanUTC = startNoEarlierThanUTC;
            }

            if (finishNoLaterThanUTC.Equals(DateTime.MinValue)) 
            {
                finishNoLaterThanUTC = DateTime.MaxValue.ToUniversalTime();
            }

            var prognoses = await _context.PrognosisCollection
                    .Find(p => (p.EmissionTimeUTC.CompareTo(startNoEarlierThanUTC) >= 0)
                              && p.EmissionTimeUTC.CompareTo(finishNoLaterThanUTC) < 0
                              && p.Region.Equals(consumptionRegion))
                    .SortBy(p => p.EmissionTimeUTC)
                    .ToListAsync();

            if (prognoses == null || prognoses.Count() == 0)
            {
                throw new NotSupportedException("No prognosis data");
            }

            var lastPrognosisTime = prognoses.Last().EmissionTimeUTC;
            var minutesLeft = (int)(lastPrognosisTime - DateTimeOffset.Now).TotalMinutes;
            if (minutesLeft < consumptionMinutes || consumptionMinutes == 0)
            {
                // not enough prognosis data to look consumptionMinutes into the future
                throw new NotSupportedException("Insufficient prognosis data");
            }

            int windowSize = (int)Math.Ceiling(consumptionMinutes / 5f);
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

                if (Math.Round((double) curTotalEmissions/windowSize,0) < Math.Round((double) minTotalEmissions/windowSize))
                {
                    minTotalEmissions = curTotalEmissions;
                    inxMinStart = inxStart;
                }
            }

            return new ConsumptionInfoMongo
            {
                FirstEmissions = initialEmissions / windowSize,
                OptimalEmissions = minTotalEmissions / windowSize,
                LastEmissions = curTotalEmissions / windowSize,
                FirstConsumptionStartUTC = prognoses.First().EmissionTimeUTC,
                OptimalConsumptionStartUTC = prognoses[inxMinStart].EmissionTimeUTC,
                LastConsumptionStartUTC = prognoses[inxStart].EmissionTimeUTC,
                ConsumptionMinutes = consumptionMinutes,
                ConsumptionRegion = consumptionRegion,
                PrognosisUpdateTimeUTC = prognoses.First().RecordedTimeUTC,
                LastPrognosisTimeUTC = prognoses.Last().EmissionTimeUTC
            };
        }

    }
}