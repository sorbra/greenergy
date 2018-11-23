using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Greenergy.Emissions.API.Server.Models.Mongo;
using Greenergy.Settings;

namespace Greenergy.Database
{
    public class EmissionDataContext: IEmissionDataContext
    {
        private readonly IMongoDatabase _database = null;

        public EmissionDataContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.Database);
            }
        }

        public IMongoCollection<EmissionDataMongo> EmissionsCollection
        {
            get
            {
                return _database.GetCollection<EmissionDataMongo>("Emissions");
            }
        }

        public IMongoCollection<PrognosisDataMongo> PrognosisCollection
        {
            get
            {
                return _database.GetCollection<PrognosisDataMongo>("Prognoses");
            }
        }
    }
}