using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Greenergy;
using Greenergy.Models;
using Greenergy.Settings;
using System.Linq;

namespace Greenergy.Database
{
    public class PrognosisRepository : IPrognosisRepository
    {
        private readonly IEmissionDataContext _context;

        public PrognosisRepository(IEmissionDataContext context)
        {
            //            _context = new EmissionDataContext(settings);
            _context = context;
        }

        public async Task UpdatePrognosisData(List<PrognosisDataMongo> prognoses)
        {
            foreach (var pg in prognoses)
            {
                var filter = Builders<PrognosisDataMongo>.Filter.Eq(pgx => pgx.Region, pg.Region)
                           & Builders<PrognosisDataMongo>.Filter.Eq(pgx => pgx.TimeStampUTC, pg.TimeStampUTC);
                var update = Builders<PrognosisDataMongo>.Update
                                .Set(pgx => pgx.Emission, pg.Emission)
                                .CurrentDate(pgx => pgx.CreatedOn);
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

        public async Task<ConsumptionInfoMongo> OptimalConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThan, DateTime finishNoLaterThan)
        {
            if (startNoEarlierThan.Equals(DateTime.MinValue))
            {
                startNoEarlierThan = DateTime.Now.ToUniversalTime();
            }
            else
            {
                startNoEarlierThan = startNoEarlierThan.ToUniversalTime();
            }

            if (finishNoLaterThan.Equals(DateTime.MinValue)) 
            {
                finishNoLaterThan = DateTime.MaxValue.ToUniversalTime();
            }

            var prognoses = await _context.PrognosisCollection
                    .Find(p => (p.TimeStampUTC.CompareTo(startNoEarlierThan) >= 0)
                              && p.TimeStampUTC.CompareTo(finishNoLaterThan) < 0
                              && p.Region.Equals(consumptionRegion))
                    .SortBy(p => p.TimeStampUTC)
                    .ToListAsync();

            if (prognoses == null || prognoses.Count() == 0)
            {
                throw new NotSupportedException("No prognosis data");
            }

            var lastPrognosisTime = prognoses.Last().TimeStampUTC;
            var minutesLeft = (int)(lastPrognosisTime - DateTime.Now).TotalMinutes;
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
                firstEmissions = initialEmissions / windowSize,
                optimalEmissions = minTotalEmissions / windowSize,
                lastEmissions = curTotalEmissions / windowSize,
                firstConsumptionStartUTC = prognoses.First().TimeStampUTC,
                optimalConsumptionStartUTC = prognoses[inxMinStart].TimeStampUTC,
                lastConsumptionStartUTC = prognoses[inxStart].TimeStampUTC,
                consumptionMinutes = consumptionMinutes,
                consumptionRegion = consumptionRegion,
                prognosisUpdateTimeUTC = prognoses.First().CreatedOn,
                lastPrognosisTimeUTC = prognoses.Last().TimeStampUTC
            };
        }

    }
}