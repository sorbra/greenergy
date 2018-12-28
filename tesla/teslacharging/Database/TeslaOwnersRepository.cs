using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Greenergy;
using Greenergy.TeslaCharger.MongoModels;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Greenergy.TeslaCharger.Registry
{
    public class TeslaOwnersRepository : ITeslaOwnersRepository
    {
        private readonly ITeslaDataContext _context = null;
        private readonly ILogger<TeslaOwnersRepository> _logger;

        public TeslaOwnersRepository(ITeslaDataContext context, ILogger<TeslaOwnersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateTeslaOwners(List<TeslaOwnerMongo> owners)
        {
            foreach (var owner in owners)
            {
                await UpdateTeslaOwner(owner);
            }
        }
        public async Task UpdateTeslaOwner(TeslaOwnerMongo owner)
        {
            var filter = Builders<TeslaOwnerMongo>.Filter.Eq(ox => ox.Email, owner.Email);
            var update = Builders<TeslaOwnerMongo>.Update
                            .Set(ox => ox.Email, owner.Email)
                            .Set(ox => ox.AccessToken, owner.AccessToken)
                            .Set(ox => ox.vehicles, owner.vehicles);
            var options = new UpdateOptions();
            options.IsUpsert = true;
            try
            {
                await _context.TeslaOwnerCollection.UpdateOneAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,ex.Message);
                throw ex;
            }
        }

        public Task<List<TeslaOwnerMongo>> GetTeslaOwners()
        {
            throw new NotImplementedException();
        }

        public Task<TeslaOwnerMongo> GetTeslaOwner()
        {
            throw new NotImplementedException();
        }
    }
}