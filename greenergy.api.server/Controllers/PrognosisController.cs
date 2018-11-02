using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.API.Models;
using Greenergy.Database;
using Greenergy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace greenergy.api.server.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PrognosisController : ControllerBase
    {
        private ILogger<PrognosisController> _logger;
        private IPrognosisRepository _prognosisRepository;

        public PrognosisController (ILogger<PrognosisController> logger, IPrognosisRepository prognosisRepository)
        {
            _logger = logger;
            _prognosisRepository = prognosisRepository;
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public async Task<ActionResult> UpdatePrognoses([FromBody] List<PrognosisDataMongo> prognoses )
        {
            await _prognosisRepository.UpdatePrognosisData(prognoses);

            _logger.LogDebug($"Received {prognoses.Count} EmissionData elements");

            return Ok();
        }

        [HttpGet("optimize")]
        public async Task<ActionResult<ConsumptionInfoDTO>> OptimalFutureConsumptionTime(int consumptionMinutes, string consumptionRegion, DateTime startNoEalierThan, DateTime finishNoLaterThan)
        {
            try
            {
                var cim =  await _prognosisRepository.OptimalFutureConsumptionTime(consumptionMinutes, consumptionRegion, startNoEalierThan, finishNoLaterThan);
                return (ConsumptionInfoDTO) cim;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in PrognosisController.OptimalFutureConsumptionTime", null);
                return null;
            }
        }
    }
}
