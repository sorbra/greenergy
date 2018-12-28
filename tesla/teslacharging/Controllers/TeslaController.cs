using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Greenergy.TeslaCharger.DTOModels;
using Greenergy.TeslaCharger.MongoModels;
using Greenergy.TeslaTools;

namespace Greenergy.TeslaCharger.Registry.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeslaController : ControllerBase
    {
        private ILogger<TeslaController> _logger;
        private ITeslaOwnersRepository _teslaRepository;

        public TeslaController(ILogger<TeslaController> logger, ITeslaOwnersRepository teslaRepository)
        {
            _logger = logger;
            _teslaRepository = teslaRepository;
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTeslaOwner([FromBody] TeslaOwnerDTO ownerDTO)
        {
            try
            {
                //var owner = new TeslaOwner(ownerDTO);
                var owner = new TeslaOwner(ownerDTO.Email,ownerDTO.AccessToken);
                var vehicles = await owner.GetVehiclesAsync();
                
                await _teslaRepository.UpdateTeslaOwner(
                    new TeslaOwnerMongo() {
                        Email = ownerDTO.Email,
                        AccessToken = ownerDTO.AccessToken,
                        vehicles = vehicles.ConvertAll<TeslaVehicleMongo>( tv => new TeslaVehicleMongo {
                            Id = tv.Id,
                            VIN = tv.VIN,
                            DisplayName = tv.DisplayName
                        })
                    }
                );
                _logger.LogInformation($"Updated {ownerDTO.Email}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<TeslaOwnerDTO>>> GetTeslaOwners()
        {
            try
            {
                var owners = await _teslaRepository.GetTeslaOwners();
                if (owners != null)
                {
                    return owners.ConvertAll(o => new TeslaOwnerDTO()
                    {
                        Email = o.Email
                    });
                }
                else
                {
                    return new List<TeslaOwnerDTO>();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
