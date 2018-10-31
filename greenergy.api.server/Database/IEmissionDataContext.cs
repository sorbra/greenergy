using Greenergy.Models;
using MongoDB.Driver;

namespace Greenergy.Database
{
    public interface IEmissionDataContext
    {
        IMongoCollection<EmissionData> EmissionsCollection { get; }

        IMongoCollection<PrognosisData> PrognosisCollection { get; }
    }
}