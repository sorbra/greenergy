using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Greenergy.Emissions.API.Server.Models.Mongo;
using Greenergy.Emissions.API.Client.Models;
using Greenergy.Emissions.Optimization;
using Greenergy.Emissions.Messaging;

namespace Greenergy.Emissions.API.Server.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PrognosisController : ControllerBase
    {
        private ILogger<PrognosisController> _logger;
        private IPrognosisRepository _prognosisRepository;
        private IEmissionsRepository _emissionsRepository;
        private IConsumptionOptimizer _optimizer;
        private IPrognosisMessageSink _messageSink;

        public PrognosisController (
            ILogger<PrognosisController> logger, 
            IPrognosisRepository prognosisRepository, 
            IEmissionsRepository emissionsRepository,
            IConsumptionOptimizer optimizer,
            IPrognosisMessageSink messageSink)
        {
            _logger = logger;
            _prognosisRepository = prognosisRepository;
            _emissionsRepository = emissionsRepository;
            _optimizer = optimizer;
            _messageSink = messageSink;
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public async Task<ActionResult> UpdatePrognoses([FromBody] List<EmissionDataDTO> prognoses)
        {
            await _prognosisRepository.UpdatePrognosisData(
                    prognoses.Select(edto => new PrognosisDataMongo( edto.Emission, edto.EmissionTimeUTC.UtcDateTime, edto.Region)).ToList()
            );

            var bestConsumption = _optimizer.SuggestConsumption(prognoses);

            await _messageSink.SendPrognoses(bestConsumption);

            _logger.LogInformation($"Received {prognoses.Count} EmissionData elements");

            return Ok();
        }

        // Set startNoEarlierThan and finishNoLaterThan to "0001-01-01" to get default return values
        [HttpGet("suggest")]
        public async Task<ActionResult<OptimalConsumptionPrognosis>> OptimalConsumptionTime(string region, int hours, string earliestConsumptionTime, string latestConsumptionTime)
        {
            var earliestConsumptionTimeUTC = string.IsNullOrEmpty(earliestConsumptionTime) ? DateTime.UtcNow : DateTime.Parse(earliestConsumptionTime).ToUniversalTime();
            var latestConsumptionTimeUTC = string.IsNullOrEmpty(latestConsumptionTime) ? DateTime.MaxValue.ToUniversalTime() : DateTime.Parse(latestConsumptionTime).ToUniversalTime();

            // var earliestConsumptionTimeUTC = DateTime.Parse(earliestConsumptionTime).ToUniversalTime();
            // var latestConsumptionTimeUTC = DateTime.Parse(latestConsumptionTime).ToUniversalTime();

            try
            {
                var prognosesMongo =  await _prognosisRepository.GetPrognoses(region, hours, earliestConsumptionTimeUTC, latestConsumptionTimeUTC);

                var prognosesDTO = prognosesMongo
                                        .Select(pm => new EmissionDataDTO( pm.Emission,  pm.EmissionTimeUTC, pm.Region))
                                        .OrderBy(pdto => pdto.EmissionTimeUTC)
                                        .ToList();

                return _optimizer.SuggestConsumption(region, hours, prognosesDTO);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
