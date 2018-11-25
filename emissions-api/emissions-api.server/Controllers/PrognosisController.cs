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
using Greenergy.Emissions.MessageProviders;

namespace Greenergy.Emissions.API.Server.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PrognosisController : ControllerBase
    {
        private ILogger<PrognosisController> _logger;
        private IPrognosisRepository _prognosisRepository;
        private IEmissionsRepository _emissionsRepository;
        private IPrognosisMessageProvider _messageProvider;

        public PrognosisController (
            ILogger<PrognosisController> logger, 
            IPrognosisRepository prognosisRepository, 
            IEmissionsRepository emissionsRepository,
            IPrognosisMessageProvider messageProvider)
        {
            _logger = logger;
            _prognosisRepository = prognosisRepository;
            _emissionsRepository = emissionsRepository;
            _messageProvider = messageProvider;
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public async Task<ActionResult> UpdatePrognoses([FromBody] List<EmissionDataDTO> prognoses )
        {
            await _prognosisRepository.UpdatePrognosisData(
                    prognoses.Select(edto => new PrognosisDataMongo( edto.Emission, edto.EmissionTimeUTC.UtcDateTime, edto.Region)).ToList()
            );

            _messageProvider.OnNewPrognosisData(prognoses);

            _logger.LogDebug($"Received {prognoses.Count} EmissionData elements");

            return Ok();
        }

        // Set startNoEarlierThan and finishNoLaterThan to "0001-01-01" to get default return values
        [HttpGet("optimize")]
        public async Task<ActionResult<ConsumptionInfoDTO>> OptimalConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTimeOffset startNoEarlierThan, DateTimeOffset finishNoLaterThan)
        {
            try
            {
                var cim =  await _prognosisRepository.OptimalConsumptionTime(consumptionMinutes, consumptionRegion, startNoEarlierThan.UtcDateTime, finishNoLaterThan.UtcDateTime);
                var ci = (ConsumptionInfoDTO) cim;
                return ci;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
