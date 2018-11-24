
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API;

namespace Greenergy.Energinet
{
    public interface IEnerginetAPI
    {
        Task<List<EmissionDataDTO>> GetRecentEmissions(DateTimeOffset noEarlierThan);
        Task<List<EmissionDataDTO>> GetCurrentEmissionsPrognosis();
    }
}