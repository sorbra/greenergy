using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.Settings;
using Greenergy.Energinet;
using System.Net.Http;
using Greenergy.Emissions.API;
using System.Linq;

namespace Greenergy.Services
{
    public class GreenergyService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IEnerginetAPI _energinetAPI;

        private Timer _emissionsSyncTimer;
        private Timer _prognosisSyncTimer;
        private HttpClient _httpClient;
        private EmissionsClient EmissionsClient
        {
            get
            {
                var client = new EmissionsClient(_httpClient);
                client.BaseUrl = _config.Value.EmissionsServiceBaseURL;
                return client;
            }
        }
        private PrognosisClient PrognosisClient
        {
            get
            {
                var client = new PrognosisClient(_httpClient);
                client.BaseUrl = _config.Value.EmissionsServiceBaseURL;
                return client;
            }
        }
        public GreenergyService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<GreenergyService> logger,
            IEnerginetAPI energinetAPI
            )
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _energinetAPI = energinetAPI;
            _httpClient = new HttpClient();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            _logger.LogDebug("Service started: " + _config.Value.Name);

            _emissionsSyncTimer = new Timer(SyncEmissionData, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.EmissionsSyncRateInMinutes * 60));
            _prognosisSyncTimer = new Timer(SyncPrognosisData, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.PrognosisSyncRateInMinutes * 60));

            _logger.LogDebug($"Emissions data will be updated from energinet.dk every {_config.Value.EmissionsSyncRateInMinutes} minutes");
            _logger.LogDebug($"Prognosis data will be updated from energinet.dk every {_config.Value.PrognosisSyncRateInMinutes} minutes");

            return Task.CompletedTask;
        }

        private async void SyncEmissionData(object state)
        {
            try
            {
                var noEarlierThan = DateTimeOffset.MinValue;
                var mostRecent = await EmissionsClient.GetMostRecentEmissionsAsync(); 
                if (mostRecent.Count() > 0)
                {
                    noEarlierThan = mostRecent.First().EmissionTimeUTC;
                }

                if (noEarlierThan.CompareTo(DateTimeOffset.MinValue) == 0)
                {
                    noEarlierThan = _config.Value.BootstrapDate;
                }

                var emissions = await _energinetAPI.GetRecentEmissions(noEarlierThan);

                _logger.LogInformation("Received " + emissions.Count + " emissions records from energinet.dk that are new since " + noEarlierThan.ToString());

                await EmissionsClient.UpdateEmissionsAsync(emissions);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in GreenergyService.SyncEmissionData", null);
            }
        }
        private async void SyncPrognosisData(object state)
        {
            try
            {
                var prognosis = await _energinetAPI.GetCurrentEmissionsPrognosis();

                _logger.LogInformation("Received " + prognosis.Count + " prognosis records from energinet.dk");

                await PrognosisClient.UpdatePrognosesAsync(prognosis);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception in GreenergyService.SyncPrognosisData", null);
            }
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
