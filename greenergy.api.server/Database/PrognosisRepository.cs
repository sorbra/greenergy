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

        public async Task UpdatePrognosisData(List<PrognosisData> prognoses)
        {
            foreach (var pg in prognoses)
            {
                var filter = Builders<PrognosisData>.Filter.Eq(pgx => pgx.Region, pg.Region)
                           & Builders<PrognosisData>.Filter.Eq(pgx => pgx.TimeStampUTC, pg.TimeStampUTC);
                var update = Builders<PrognosisData>.Update
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


        public async Task<ConsumptionInfo> OptimalFutureConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime finishNoLaterThan)
        {
            var prognoses = await _context.PrognosisCollection
                    .Find(p => (p.TimeStampUTC.CompareTo(DateTime.Now) >= 0)
                              && p.TimeStampUTC.CompareTo(finishNoLaterThan) < 0
                              && p.Region.Equals(consumptionRegion))
                    .SortBy(p => p.TimeStampUTC)
                    .ToListAsync();

            if (prognoses == null || prognoses.Count() == 0)
            {
                throw new NotSupportedException("No matching prognosis data");
            }

            var lastPrognosisTime = prognoses[prognoses.Count() - 1].TimeStampUTC;
            var minutesLeft = (int)(lastPrognosisTime - DateTime.Now).TotalMinutes;
            if (minutesLeft < consumptionMinutes || consumptionMinutes == 0)
            {
                // not enough prognosis data to look consumptionMinutes into the future
                throw new NotSupportedException("Insufficient prognosis data");
            }

            int windowSize = (int)Math.Ceiling(consumptionMinutes / 5f);
            var minTotalEmission = prognoses.Take(windowSize).Sum(p => p.Emission);
            var curTotalEmission = minTotalEmission;

            int inxStart = 0;
            int inxMinStart = 0;
            int inxEnd = windowSize;
            int count = prognoses.Count();

            while (inxEnd < count)
            {
                curTotalEmission = curTotalEmission + prognoses[inxEnd].Emission - prognoses[inxStart].Emission;

                inxEnd++; inxStart++;

                if (curTotalEmission < minTotalEmission)
                {
                    minTotalEmission = curTotalEmission;
                    inxMinStart = inxStart;
                }
            }

            return new ConsumptionInfo
            {
                co2perkwh = minTotalEmission / windowSize,
                consumptionStart = prognoses[inxMinStart].TimeStampUTC,
                consumptionMinutes = consumptionMinutes,
                consumptionRegion = consumptionRegion,
                lastPrognosisUpdateTime = prognoses[0].CreatedOn
            };
        }
    }
}