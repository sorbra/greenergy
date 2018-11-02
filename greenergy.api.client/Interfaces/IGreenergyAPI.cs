using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.API.Models;

namespace Greenergy.API
{
    public interface IGreenergyAPI    { 
        /* Saves new emissions data to the database. Existing data in the database with same timestamp and region will get overwritten. */
        Task UpdateEmissions(List<EmissionDataDTO> emissions);
        /* Returns the DateTime of the most recent emissions in the database. */
        Task<DateTime> GetMostRecentEmissionsTimeStamp();
        /* Returns the Emissions Data in the database with the most recent timestamp. */
        Task<List<EmissionDataDTO>> GetMostRecentEmissions();
        Task UpdateEmissionsPrognosis(List<EmissionDataDTO> prognosis);
        Task<ConsumptionInfoDTO> OptimalFutureConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEarlierThan, DateTime finishNoLaterThan);
    }
}