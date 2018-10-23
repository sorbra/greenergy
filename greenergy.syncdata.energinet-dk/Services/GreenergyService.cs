using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.Models;
using Greenergy.Settings;
using Greenergy.Database;
using Greenergy.Energinet;

namespace Greenergy.Services
{
    public class GreenergyService : IHostedService, IDisposable
    {
        private readonly IOptions<Application> _config;
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private Timer _timer;
        private readonly IEmissionsRepository _emissionsRepository;
        
        public GreenergyService(
            IOptions<Application> config,
            IApplicationLifetime applicationLifetime,
            ILogger<GreenergyService> logger,
            IEmissionsRepository emissionsRepository)
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _emissionsRepository = emissionsRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            var name = _config.Value.Name;
            _logger.LogInformation("Service started: " + name);

            _timer = new Timer(SyncData, null, TimeSpan.Zero, TimeSpan.FromSeconds(60*60));

            return Task.CompletedTask;
        }

        private void SyncData(object state)
        {
            var noEarlierThan = _emissionsRepository.MostRecentEmissionDataTimeStamp().Result;
            var emissions = EnerginetFacade.GetRecentEmissions(noEarlierThan).Result;
            _emissionsRepository.UpdateEmissionData(emissions);

            _logger.LogInformation(DateTime.Now + ": Received " + emissions.Count + " records from energinet since " + noEarlierThan.ToString());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            _timer.Change(Timeout.Infinite,0);
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
