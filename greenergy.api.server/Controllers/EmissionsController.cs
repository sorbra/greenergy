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

        public EmissionsController (ILogger<EmissionsController> logger, IEmissionsRepository emissionsRepository)
        {
            _logger = logger;
            _emissionsRepository = emissionsRepository;
        }

        // GET api/values
        // [HttpGet]
        // public ActionResult<IEnumerable<EmissionData>> Get(int hours = 1)
        // {
        //     var emissions = _emissionsRepository.GetRecentEmissionData(hours).Result as List<EmissionData>;
        //     return emissions.OrderByDescending(e => e.TimeStampUTC).ToList();
        // }

        [HttpGet("{when}")]
        public ActionResult<List<EmissionData>> GetMostRecentEmissions(string when)
        {
            if (when.ToLower().Equals("latest"))
            {
                return _emissionsRepository.GetLatest().Result;
            }
            return null;
        }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public ActionResult UpdateEmissions([FromBody] List<EmissionData> emissions )
        {
            _emissionsRepository.UpdateEmissionData(emissions);

            _logger.LogDebug($"Received {emissions.Count} EmissionData elements");

            return Ok();
        }
    }
}
