using Greenergy.Models;
using MongoDB.Driver;

namespace Greenergy.Database
{
    public interface IEmissionDataContext
    {
        IMongoCollection<EmissionDataMongo> EmissionsCollection { get; }

        IMongoCollection<PrognosisDataMongo> PrognosisCollection { get; }
    }
}