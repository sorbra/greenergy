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
    public class TeslaVehicleController : ControllerBase
    {
        private ILogger<TeslaVehicleController> _logger;
        private ITeslaVehiclesRepository _teslaRepository;

        public TeslaVehicleController(ILogger<TeslaVehicleController> logger, ITeslaVehiclesRepository teslaRepository)
        {
            _logger = logger;
            _teslaRepository = teslaRepository;
        }
        [HttpPost]
        public async Task<ActionResult> RegisterVehicle([FromBody] TeslaVehicleDTO vehicleReceived)
        {
            try
            {
                var owner = new TeslaOwner(vehicleReceived.OwnerEmail, vehicleReceived.AccessToken);
                var vehicles = await owner.GetVehiclesAsync();

                var vehicleFound = vehicles.Where(v => v.VIN == vehicleReceived.VIN).FirstOrDefault();
                if (vehicleFound == null)
                {
                    var msg = $"User {vehicleReceived.OwnerEmail} does not own vehicle {vehicleReceived.VIN}";
                    _logger.LogWarning(msg);
                    return StatusCode(StatusCodes.Status403Forbidden, msg);
                }

                await _teslaRepository.UpdateTeslaVehicle(
                    new TeslaVehicleMongo()
                    {
                        OwnerEmail = vehicleReceived.OwnerEmail,
                        AccessToken = vehicleReceived.AccessToken,
                        Id = vehicleFound.Id,
                        VIN = vehicleFound.VIN,
                        DisplayName = vehicleFound.DisplayName,
                        ChargingConstraints = vehicleReceived.ChargingConstraints.ConvertAll(cc =>
                            new ChargingConstraintMongo()
                            {
                                WeekDays = Array.ConvertAll(cc.WeekDays, ds => Enum.Parse<DayOfWeek>(ds)),
                                Date = cc.Date,
                                ByHour = cc.ByHour,
                                NoEarlierThanHour = cc.NoEarlierThanHour,
                                TimeZone = cc.TimeZone,
                                MinCharge = cc.MinCharge,
                                MaxCharge = cc.MaxCharge
                            }
                        )
                    }
                );
                _logger.LogInformation($"Registered vehicle {vehicleFound.VIN}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
