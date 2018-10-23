using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Greenergy.Models;

namespace Greenergy.Database
{
    public class EmissionDataContext
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

        public IMongoCollection<EmissionData> EmissionsCollection
        {
            get
            {
                return _database.GetCollection<EmissionData>("Emissions");
            }
        }
    }
}