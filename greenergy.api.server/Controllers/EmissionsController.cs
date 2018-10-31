using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Database;
using Greenergy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace greenergy.api.server.Controllers
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

        [HttpGet("{when}")]
        public async Task<ActionResult<List<EmissionData>>> GetMostRecentEmissions(string when)
        {
            try
            {
                if (when.ToLower().Equals("latest"))
                {
                    return await _emissionsRepository.GetLatest();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in EmissionsController.GetMostRecentEmissions", null);
            }
            return null;
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public async Task<ActionResult> UpdateEmissions([FromBody] List<EmissionData> emissions)
        {
            try
            {
                await _emissionsRepository.UpdateEmissionData(emissions);

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
