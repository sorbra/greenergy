using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Greenergy.Emissions.API.Client.Models;
using Greenergy.Emissions.API.Server.Models.Mongo;

namespace Greenergy.Emissions.API.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmissionsController : ControllerBase
    {
        private ILogger<EmissionsController> _logger;
        private IEmissionsRepository _emissionsRepository;

        public EmissionsController(ILogger<EmissionsController> logger, IEmissionsRepository emissionsRepository)
        {
            _logger = logger;
            _emissionsRepository = emissionsRepository;
        }

        [HttpGet("latest")]
        public async Task<ActionResult<List<EmissionDataDTO>>> GetMostRecentEmissions()
        {
            try
            {
                var latest = await _emissionsRepository.GetLatest();
                if (latest != null)
                {
                    return latest.ConvertAll(m => (EmissionDataDTO) m);
                }
                else
                {
                    return new List<EmissionDataDTO>();
                }
                
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in EmissionsController.GetMostRecentEmissions", null);
                return null;
            }
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public async Task<ActionResult> UpdateEmissions([FromBody] List<EmissionDataDTO> emissions)
        {
            try
            {
                await _emissionsRepository.UpdateEmissionData(
                    emissions.Select(edto => new EmissionDataMongo( edto.Emission, edto.EmissionTimeUTC.UtcDateTime, edto.Region)).ToList()
                );

                _logger.LogDebug($"Received {emissions.Count} EmissionData elements");

                return Ok();

            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Exception in EmissionsController.UpdateEmissions", null);
            }
            return null;
        }
    }
}
