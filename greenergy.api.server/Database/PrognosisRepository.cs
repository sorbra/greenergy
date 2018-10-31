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

        public async Task<List<PrognosisData>> PrognosisMinimum()
        {
            try
            {
                var now = DateTime.Now;
                var min = await _context.PrognosisCollection
                        .Find(x => x.TimeStampUTC.CompareTo(now) >= 0)
                        .SortBy(x => x.Emission)
                        .Limit(10)
                        .FirstOrDefaultAsync();
                if (min != null)
                {
                    return await _context.PrognosisCollection
                            .Find(x => x.Emission == min.Emission)
                            .ToListAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

    }
}