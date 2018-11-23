using Greenergy.Emissions.API.Server.Models.Mongo;
using MongoDB.Driver;

namespace Greenergy.Database
{
    public interface IEmissionDataContext
    {
        IMongoCollection<EmissionDataMongo> EmissionsCollection { get; }

        IMongoCollection<PrognosisDataMongo> PrognosisCollection { get; }
    }
}