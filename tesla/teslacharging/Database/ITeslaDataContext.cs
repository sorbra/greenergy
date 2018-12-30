using Greenergy.TeslaCharger.MongoModels;
using MongoDB.Driver;

namespace Greenergy.TeslaCharger.Registry
{
    public interface ITeslaDataContext
    {
        IMongoCollection<TeslaVehicleMongo> TeslaVehicleCollection { get; }
    }
}