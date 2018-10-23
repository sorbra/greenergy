using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Greenergy;
using Greenergy.Models;
using System.Linq;

namespace Greenergy.Database
{
    public class MongoEmissionsRepository : IEmissionsRepository
    {
        private readonly EmissionDataContext _context = null;

        public MongoEmissionsRepository(IOptions<MongoSettings> settings)
        {
            _context = new EmissionDataContext(settings);
        }

        public async Task<IEnumerable<EmissionData>> GetRecentEmissionData(int hours)
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

        public async Task<List<EmissionData>> GetEmissionDataSince(DateTime noEarlierThan)
        {
            try
            {
                return await _context.EmissionsCollection
                        .Find(x => x.TimeStampUTC.CompareTo(noEarlierThan) > 0)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }


        public async Task InsertEmissionData(List<EmissionData> emissions)
        {
            try
            {
                await _context.EmissionsCollection.InsertManyAsync(emissions);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<List<EmissionData>> GetEmissionData()
        {
            try
            {
                return await _context.EmissionsCollection
                    .Find(_ => true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task InsertOrUpdateEmissionData(List<EmissionData> emissions)
        {
            foreach (var ed in emissions)
            {
                var filter = Builders<EmissionData>.Filter.Eq(edx => edx.Region, ed.Region)
                           & Builders<EmissionData>.Filter.Eq(edx => edx.TimeStampUTC, ed.TimeStampUTC);
                var update = Builders<EmissionData>.Update
                                .Set(edx => edx.Emission, ed.Emission)
                                .CurrentDate(edx => edx.CreatedOn);
                var options = new UpdateOptions();
                options.IsUpsert = true;
                try
                {
                    await _context.EmissionsCollection.UpdateOneAsync(filter,update,options);
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
                var lastRecord = await _context.EmissionsCollection
                            .Find(_ => true)
                            .Limit(1)
                            .Sort(new BsonDocument("TimeStampUTC", -1))
                            .FirstAsync();
                
                return lastRecord.TimeStampUTC;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}