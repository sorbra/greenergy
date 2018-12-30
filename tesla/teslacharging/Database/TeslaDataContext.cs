using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Greenergy.TeslaCharger.MongoModels;
using Greenergy.TeslaCharger.Settings;

namespace Greenergy.TeslaCharger.Registry
{
    public class TeslaDataContext: ITeslaDataContext
    {
        private readonly IMongoDatabase _database = null;

        public TeslaDataContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.Database);
            }
        }
        public IMongoCollection<TeslaVehicleMongo> TeslaVehicleCollection
        {
            get
            {
                return _database.GetCollection<TeslaVehicleMongo>("TeslaVehicles");
            }
        }
    }
}