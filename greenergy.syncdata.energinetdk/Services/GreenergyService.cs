using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.Settings;
using Greenergy.Energinet;
using Greenergy.API;

namespace Greenergy.Services
{
    public class GreenergyService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IGreenergyAPIClient _energyDataClient;
        private Timer _timer;

        public GreenergyService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<GreenergyService> logger,
            IGreenergyAPIClient energyDataClient
            )
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _energyDataClient = energyDataClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            var name = _config.Value.Name;
            _logger.LogInformation("Service started: " + name);

            _timer = new Timer(SyncData, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.UpdateDelayInMinutes * 60));

            _logger.LogInformation($"Data will be updated from energinet.dk every {_config.Value.UpdateDelayInMinutes} minutes");

            return Task.CompletedTask;
        }

        private void SyncData(object state)
        {
            var noEarlierThan = _energyDataClient.GetLatestTimeStamp().Result;

            if (noEarlierThan.CompareTo(DateTime.MinValue) == 0)
            {
                noEarlierThan = _config.Value.BootstrapDate;
            }

            var emissions = EnerginetAPI.GetRecentEmissions(noEarlierThan).Result;

            _logger.LogInformation(DateTime.Now + ": Received " + emissions.Count + " records from energinet since " + noEarlierThan.ToString());

            _energyDataClient.UpdateEmissionData(emissions).Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
