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
    public class EmissionsRepository : IEmissionsRepository
    {
        private readonly IEmissionDataContext _context = null;

        public EmissionsRepository(IEmissionDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmissionDataMongo>> GetRecentEmissionData(int hours)
        {
            DateTime startTime = DateTime.Now.AddHours(-hours);

            try
            {
                return await _context.EmissionsCollection
                        .Find(x => x.TimeStampUTC.CompareTo(startTime) > 0)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<List<EmissionDataMongo>> GetEmissionDataSince(DateTime noEarlierThan)
        {
            try
            {
                return await _context.EmissionsCollection
                        .Find(x => x.TimeStampUTC.CompareTo(noEarlierThan) >= 0)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task UpdateEmissionData(List<EmissionDataMongo> emissions)
        {
            foreach (var ed in emissions)
            {
                var filter = Builders<EmissionDataMongo>.Filter.Eq(edx => edx.Region, ed.Region)
                           & Builders<EmissionDataMongo>.Filter.Eq(edx => edx.TimeStampUTC, ed.TimeStampUTC);
                var update = Builders<EmissionDataMongo>.Update
                                .Set(edx => edx.Emission, ed.Emission)
                                .CurrentDate(edx => edx.CreatedOn);
                var options = new UpdateOptions();
                options.IsUpsert = true;
                try
                {
                    await _context.EmissionsCollection.UpdateOneAsync(filter, update, options);
                }
                catch (Exception ex)
                {
                    // log or manage the exception
                    throw ex;
                }
            }

        }

        public async Task<DateTime> MostRecentEmissionDataTimeStamp()
        {
            try
            {
                var query = _context.EmissionsCollection
                            .Find(_ => true)
                            .Limit(1)
                            .Sort(new BsonDocument("TimeStampUTC", -1));
                var lastRecord = await query.FirstOrDefaultAsync();
                if (lastRecord == null)
                {
                    return DateTime.MinValue;
                }
                return lastRecord.TimeStampUTC;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<List<EmissionDataMongo>> GetLatest()
        {
            DateTime latestTime = await MostRecentEmissionDataTimeStamp();
            if (latestTime.CompareTo(DateTime.MinValue) == 0)
            {
                return null;
            }
            try
            {
                return await _context.EmissionsCollection
                            .Find(ed => ed.TimeStampUTC.CompareTo(latestTime) == 0)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}