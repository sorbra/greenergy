using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.TeslaCharger.MongoModels;

namespace Greenergy.TeslaCharger.Registry
{
    public interface ITeslaVehiclesRepository
    {
//        Task<List<TeslaVehicleMongo>> GetTeslaVehicles();
        Task UpdateTeslaVehicle(TeslaVehicleMongo owner);
    }
}