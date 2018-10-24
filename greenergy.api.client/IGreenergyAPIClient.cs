using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greenergy.API
{
    public interface IGreenergyAPIClient
    { 
        Task UpdateEmissionData(List<EmissionDataDTO> emissions);
        
        Task<List<EmissionDataDTO>> GetLatest();
        Task<DateTime> GetLatestTimeStamp();
    }
}