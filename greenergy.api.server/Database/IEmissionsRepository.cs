using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Models;

namespace Greenergy.Database
{
    public interface IEmissionsRepository
    {
        Task<IEnumerable<EmissionData>> GetRecentEmissionData(int hours);
        Task<List<EmissionData>> GetEmissionDataSince(DateTime noEarlierThan);
        // Insert emission data. Existing emission data that match on TimeStampUTC and Region will get updated.
        Task UpdateEmissionData(List<EmissionData> emissions);
        Task<DateTime> MostRecentEmissionDataTimeStamp();
        Task<List<EmissionData>> GetLatest();
    }
}