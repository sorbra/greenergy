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

namespace Greenergy.TeslaCharger.Service
{
    public class TeslaChargerService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly ITeslaDataContext _teslactx;
        private readonly IApplicationLifetime _applicationLifetime;

        private Timer _chargingCheckTimer;
        public TeslaChargerService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<TeslaChargerService> logger,
            ITeslaDataContext teslactx
            )
        {
            _config = config;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _teslactx = teslactx;
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
            try
            {
                var owners = _teslactx.TeslaVehicleCollection;
                var filter = Builders<TeslaVehicleMongo>.Filter.Empty;

                _logger.LogInformation("CheckCharging called...");
//                await (await owners.FindAsync(filter)).ForEachAsync( to => _logger.LogInformation(to.Email));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in TeslaChargerService.CheckCharging", null);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping");
            _chargingCheckTimer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void OnStarted()
        {
            _logger.LogDebug("OnStarted has been called.");

            // Perform post-startup activities here
        }
        private void OnStopping()
        {
            _logger.LogDebug("OnStopping has been called.");

            // Perform on-stopping activities here
        }
        private void OnStopped()
        {
            _logger.LogDebug("OnStopped has been called.");

            // Perform post-stopped activities here
        }
        public void Dispose()
        {
            _chargingCheckTimer?.Dispose();
        }
    }
}
