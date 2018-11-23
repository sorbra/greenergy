using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Server.Models.Mongo;

namespace Greenergy.Database
{
    public interface IEmissionsRepository
    {
        Task<IEnumerable<EmissionDataMongo>> GetRecentEmissionData(int hours);
        Task<List<EmissionDataMongo>> GetEmissionDataSince(DateTime noEarlierThan);
        // Insert emission data. Existing emission data that match on TimeStampUTC and Region will get updated.
        Task UpdateEmissionData(List<EmissionDataMongo> emissions);
        Task<DateTime> MostRecentEmissionDataTimeStamp();
        Task<List<EmissionDataMongo>> GetLatest();
    }
}