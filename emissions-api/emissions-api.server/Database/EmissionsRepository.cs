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
using Microsoft.Extensions.Logging;

namespace Greenergy.Database
{
    public class EmissionsRepository : IEmissionsRepository
    {
        private readonly IEmissionDataContext _context = null;
        private readonly ILogger<EmissionsRepository> _logger;

        public EmissionsRepository(IEmissionDataContext context, ILogger<EmissionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // public async Task<IEnumerable<EmissionDataMongo>> GetRecentEmissionData(int hours)
        // {
        //     DateTimeOffset startTime = DateTimeOffset.Now.AddHours(-hours);

        //     try
        //     {
        //         return await _context.EmissionsCollection
        //                 .Find(x => x.EmissionTimeUTC.CompareTo(startTime) > 0)
        //                 .ToListAsync();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogCritical(ex,ex.Message);
        //         throw ex;
        //     }
        // }

        // public async Task<List<EmissionDataMongo>> GetEmissionDataSince(DateTime noEarlierThanUTC)
        // {
        //     try
        //     {
        //         return await _context.EmissionsCollection
        //                 .Find(x => x.EmissionTimeUTC.CompareTo(noEarlierThanUTC) >= 0)
        //                 .ToListAsync();
        //     }
        //     catch (Exception ex)
        //     {
        //         // log or manage the exception
        //         throw ex;
        //     }
        // }

        public async Task UpdateEmissionData(List<EmissionDataMongo> emissions)
        {
            foreach (var ed in emissions)
            {
                var filter = Builders<EmissionDataMongo>.Filter.Eq(edx => edx.Region, ed.Region)
                           & Builders<EmissionDataMongo>.Filter.Eq(edx => edx.EmissionTimeUTC, ed.EmissionTimeUTC);
                var update = Builders<EmissionDataMongo>.Update
                                .Set(edx => edx.Region, ed.Region)
                                .Set(edx => edx.Emission, ed.Emission)
                                .Set(edx => edx.EmissionTimeUTC, ed.EmissionTimeUTC)
                                .Set(edx => edx.RecordedTimeUTC, DateTime.UtcNow);
                var options = new UpdateOptions();
                options.IsUpsert = true;
                try
                {
                    await _context.EmissionsCollection.UpdateOneAsync(filter, update, options);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex,ex.Message);
                    throw ex;
                }
            }

        }

        public async Task<DateTime> MostRecentEmissionTimeUTC()
        {
            try
            {
                var query = _context.EmissionsCollection
                            .Find(_ => true)
                            .Limit(1)
                            .Sort(new BsonDocument("EmissionTimeUTC", -1));
                var lastRecord = await query.FirstOrDefaultAsync();
                if (lastRecord == null)
                {
                    return DateTime.MinValue;
                }
                return lastRecord.EmissionTimeUTC;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<List<EmissionDataMongo>> GetLatest()
        {
            DateTime latestTimeUTC = await MostRecentEmissionTimeUTC();
            if (latestTimeUTC.CompareTo(DateTime.MinValue) == 0)
            {
                return null;
            }
            try
            {
                return await _context.EmissionsCollection
                            .Find(ed => ed.EmissionTimeUTC.CompareTo(latestTimeUTC) == 0)
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