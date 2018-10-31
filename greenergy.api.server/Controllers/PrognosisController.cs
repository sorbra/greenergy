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
    public class PrognosisController : ControllerBase
    {
        private ILogger<PrognosisController> _logger;
        private IPrognosisRepository _prognosisRepository;

        public PrognosisController (ILogger<PrognosisController> logger, IPrognosisRepository prognosisRepository)
        {
            _logger = logger;
            _prognosisRepository = prognosisRepository;
        }

        // GET api/values
        // [HttpGet]
        // public ActionResult<IEnumerable<EmissionData>> Get(int hours = 1)
        // {
        //     var emissions = _emissionsRepository.GetRecentEmissionData(hours).Result as List<EmissionData>;
        //     return emissions.OrderByDescending(e => e.TimeStampUTC).ToList();
        // }

        // Saves EmissionData  to the database. Existing data with same timestamp and region will get overwritten.
        [HttpPost]
        public ActionResult UpdatePrognoses([FromBody] List<EmissionData> prognoses )
        {
            _prognosisRepository.UpdatePrognosisData(prognoses);

            _logger.LogDebug($"Received {prognoses.Count} EmissionData elements");

            return Ok();
        }
    }
}
