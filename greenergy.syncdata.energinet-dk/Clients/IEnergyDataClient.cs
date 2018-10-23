using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Models;

namespace Greenergy.Clients
{
    public interface IEnergyDataClient
    { 
        Task UpdateEmissionData(List<EmissionData> emissions);
        
        Task<List<EmissionData>> GetLatest();
        Task<DateTime> GetLatestTimeStamp();
    }
}