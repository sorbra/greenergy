using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.Settings;
using Greenergy.Energinet;
using Greenergy.Emissions.API;
using System.Net.Http;

namespace Greenergy.Services
{
    public class OptimizerService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _applicationLifetime;

        private Timer _runOptimizerTimer;
        private TimeZoneInfo _copenhagenTimeZoneInfo;
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
        public OptimizerService(
            IOptions<ApplicationSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<OptimizerService> logger
        )
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _httpClient = new HttpClient();

            try
            {
                _copenhagenTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_config.Value.TimeZone);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                throw (ex);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            _runOptimizerTimer = new Timer(EvaluatePrognosis, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.RunEveryMinutes * 60));

            _logger.LogDebug($"Optimizer will run every {_config.Value.RunEveryMinutes} minutes");

            return Task.CompletedTask;
        }

        private async void EvaluatePrognosis(object state)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping");
            _runOptimizerTimer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogDebug("OnStarted has been called.");
        }

        private void OnStopping()
        {
            _logger.LogDebug("OnStopping has been called.");
        }

        private void OnStopped()
        {
            _logger.LogDebug("OnStopped has been called.");
        }

        public void Dispose()
        {
            _runOptimizerTimer?.Dispose();
        }
    }
}
