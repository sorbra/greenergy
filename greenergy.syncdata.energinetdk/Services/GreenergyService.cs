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
        private readonly IEnerginetAPI _energinetAPI;
        private readonly IGreenergyAPI _greenergyAPI;

        private Timer _emissionsSyncTimer;
        private Timer _prognosisSyncTimer;

        public GreenergyService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<GreenergyService> logger,
            IEnerginetAPI energinetAPI,
            IGreenergyAPI greenergyAPI
            )
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _energinetAPI = energinetAPI;
            _greenergyAPI = greenergyAPI;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            _logger.LogDebug("Service started: " +  _config.Value.Name);

            _emissionsSyncTimer = new Timer(SyncEmissionData, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.EmissionsSyncRateInMinutes * 60));
            _prognosisSyncTimer = new Timer(SyncPrognosisData, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.PrognosisSyncRateInMinutes * 60));

            _logger.LogDebug($"Emissions data will be updated from energinet.dk every {_config.Value.EmissionsSyncRateInMinutes} minutes");
            _logger.LogDebug($"Prognosis data will be updated from energinet.dk every {_config.Value.PrognosisSyncRateInMinutes} minutes");

            return Task.CompletedTask;
        }

        private void SyncEmissionData(object state)
        {
            var noEarlierThan = _greenergyAPI.GetMostRecentEmissionsTimeStamp().Result;

            if (noEarlierThan.CompareTo(DateTime.MinValue) == 0)
            {
                noEarlierThan = _config.Value.BootstrapDate;
            }

            var emissions = _energinetAPI.GetRecentEmissions(noEarlierThan).Result;

            _logger.LogInformation("Received " + emissions.Count + " emissions records from energinet.dk that are new since " + noEarlierThan.ToString());

            _greenergyAPI.UpdateEmissions(emissions).Wait();
        }
        private void SyncPrognosisData(object state)
        {
            var prognosis = _energinetAPI.GetCurrentEmissionsPrognosis().Result;

            _logger.LogInformation("Received " + prognosis.Count + " prognosis records from energinet.dk");

            _greenergyAPI.UpdateEmissionsPrognosis(prognosis).Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping");
            _emissionsSyncTimer.Change(Timeout.Infinite, 0);
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
            _emissionsSyncTimer?.Dispose();
        }
    }
}
