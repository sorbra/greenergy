using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.TeslaCharger.Registry;
using Greenergy.TeslaCharger.Settings;
using MongoDB.Driver.Linq;
using Greenergy.TeslaCharger.MongoModels;
using MongoDB.Driver;
using Greenergy.TeslaCharger.Constraints;
using Greenergy.TeslaTools;

namespace Greenergy.TeslaCharger.Service
{
    public class TeslaChargerService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly ITeslaVehiclesRepository _vehicles;
        private readonly IApplicationLifetime _applicationLifetime;

        private Timer _chargingCheckTimer;
        public TeslaChargerService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<TeslaChargerService> logger,
            ITeslaVehiclesRepository vehicles
            )
        {
            _config = config;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _vehicles = vehicles;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            _logger.LogDebug("Service started: " + _config.Value.Name);

            _chargingCheckTimer = new Timer(CheckCharging, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.ChargingCheckRateInMinutes * 60));

            _logger.LogDebug($"Charging status will be checked every {_config.Value.ChargingCheckRateInMinutes} minutes");

            return Task.CompletedTask;
        }

        private async void CheckCharging(object state)
        {
            _logger.LogDebug("CheckCharging called");
            try
            {
                var vehicles = await _vehicles.AllVehicles();
                foreach (var vehicle in vehicles)
                {
                    try
                    {
                        var chargeTimeRange = ChargeTimeRange.NextChargeBy(vehicle.ChargingConstraints);
                        TeslaVehicle v = (TeslaVehicle)vehicle;
                        var vs = await v.GetChargeStateAsync();
                        var plugged_in_text = "";
                        switch (vs.ChargingState)
                        {
                            case "Disconnected": plugged_in_text = "NOT plugged in"; break;
                            case "Stopped": plugged_in_text = "plugged in but NOT charging"; break;
                            case "Charging": plugged_in_text = "charging"; break;
                            default: plugged_in_text = vs.ChargingState; break;
                        }

                        _logger.LogInformation($"{vehicle.DisplayName} ({vehicle.VIN}) is currently {plugged_in_text}, and is charged to {vs.BatteryLevel}%.");
                        _logger.LogInformation($"Next charge time is between {chargeTimeRange.ChargeNoEarlierThan} and {chargeTimeRange.ChargeBy}, and should be charged to at least {chargeTimeRange.Constraint.MinCharge}% and at most {chargeTimeRange.Constraint.MaxCharge}%");
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //            _logger.LogDebug("Stopping");
            _chargingCheckTimer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void OnStarted()
        {
            //            _logger.LogDebug("OnStarted has been called.");

            // Perform post-startup activities here
        }
        private void OnStopping()
        {
            //            _logger.LogDebug("OnStopping has been called.");

            // Perform on-stopping activities here
        }
        private void OnStopped()
        {
            //            _logger.LogDebug("OnStopped has been called.");

            // Perform post-stopped activities here
        }
        public void Dispose()
        {
            _chargingCheckTimer?.Dispose();
        }
    }
}
