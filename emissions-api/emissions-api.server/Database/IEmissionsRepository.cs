using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Server.Models.Mongo;

namespace Greenergy.Database
{
    public interface IEmissionsRepository
    {
        // Task<IEnumerable<EmissionDataMongo>> GetRecentEmissionData(int hours);
        // Task<List<EmissionDataMongo>> GetEmissionDataSince(DateTime noEarlierThanUTC);
        // Insert emission data. Existing emission data that match on EmissionTimeUTC and Region will get updated.
        Task UpdateEmissionData(List<EmissionDataMongo> emissions);
        Task<DateTime> MostRecentEmissionTimeUTC();
        Task<List<EmissionDataMongo>> GetLatest();
    }
}