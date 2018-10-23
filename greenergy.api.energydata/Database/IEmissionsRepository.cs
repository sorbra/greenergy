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
        // Add new EmissionData to database. No attempt to merge or overwrite
        Task InsertEmissionData(List<EmissionData> emissions);
        // Insert emission data. Existing emission data that match on TimeStampUTC and Region will get updated.
        Task UpdateEmissionData(List<EmissionData> emissions);
        Task<List<EmissionData>> GetEmissionData();
        Task<DateTime> MostRecentEmissionDataTimeStamp();
        Task<List<EmissionData>> GetLatest();
    }
}