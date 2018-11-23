
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Energinet
{
    public interface IEnerginetAPI
    {
        Task<List<EmissionDataDTO>> GetRecentEmissions(DateTime noEarlierThan);
        Task<List<EmissionDataDTO>> GetCurrentEmissionsPrognosis();
    }
}