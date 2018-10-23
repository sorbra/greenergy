using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Database;
using Greenergy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace greenergy.api.energydata.Controllers
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
        [HttpGet]
        public ActionResult<IEnumerable<EmissionData>> Get(int hours = 1)
        {
            var emissions = _emissionsRepository.GetRecentEmissionData(hours).Result as List<EmissionData>;
            return emissions.OrderByDescending(e => e.TimeStampUTC).ToList();
        }

        [HttpGet("{when}")]
        public ActionResult<List<EmissionData>> GetLatest(string when)
        {
            if (when.ToLower().Equals("latest"))
            {
                return _emissionsRepository.GetLatest().Result;
            }
            return null;
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] List<EmissionData> emissions )
        {
            var headers = Request.Headers;
            _emissionsRepository.UpdateEmissionData(emissions);

            _logger.LogInformation($"Received {emissions.Count} EmissionData elements");

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
