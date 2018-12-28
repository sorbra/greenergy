using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.TeslaCharger.MongoModels;

namespace Greenergy.TeslaCharger.Registry
{
    public interface ITeslaOwnersRepository
    {
        Task<List<TeslaOwnerMongo>> GetTeslaOwners();
        Task UpdateTeslaOwners(List<TeslaOwnerMongo> owners);
        Task UpdateTeslaOwner(TeslaOwnerMongo owner);
    }
}