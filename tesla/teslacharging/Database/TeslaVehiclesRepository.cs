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
    public class TeslaVehiclesRepository : ITeslaVehiclesRepository
    {
        private readonly ITeslaDataContext _context = null;
        private readonly ILogger<ITeslaVehiclesRepository> _logger;

        public TeslaVehiclesRepository(ITeslaDataContext context, ILogger<ITeslaVehiclesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task UpdateTeslaVehicle(TeslaVehicleMongo vehicle)
        {
            var filter = Builders<TeslaVehicleMongo>.Filter.Eq(v => v.Id, vehicle.Id);
            var update = Builders<TeslaVehicleMongo>.Update
                            .Set(v => v.OwnerEmail, vehicle.OwnerEmail)
                            .Set(v => v.AccessToken, vehicle.AccessToken)
                            .Set(v => v.Id, vehicle.Id)
                            .Set(v => v.VIN, vehicle.VIN)
                            .Set(v => v.DisplayName, vehicle.DisplayName)
                            .Set(v => v.ChargingConstraints, vehicle.ChargingConstraints);
            var options = new UpdateOptions();
            options.IsUpsert = true;
            try
            {
                await _context.TeslaVehicleCollection.UpdateOneAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,ex.Message);
                throw ex;
            }
        }
    }
}