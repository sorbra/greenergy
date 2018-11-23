using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Greenergy.Settings;
using Greenergy.Energinet;
using Greenergy.Emissions.API.Client;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Services
{
    public class OptimizerService : IHostedService, IDisposable
    {
        private readonly IOptions<OptimizerSettings> _config;
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IGreenergyAPI _greenergyAPI;

        private Timer _runOptimizerTimer;
        private TimeZoneInfo _copenhagenTimeZoneInfo;

        public OptimizerService(
            IOptions<OptimizerSettings> config,
            IApplicationLifetime applicationLifetime,
            ILogger<OptimizerService> logger,
            IGreenergyAPI greenergyAPI
        )
        {
            _config = config;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _greenergyAPI = greenergyAPI;

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
                // For now, always look for the optimal time between midnight and 6AM.
                // This will need to be configurable by device/person, but for now this default
                // suits me, since I get cheaper electricity in this time interval

                var nowUTC = DateTime.Now.ToUniversalTime();
                var now = TimeZoneInfo.ConvertTime(nowUTC, _copenhagenTimeZoneInfo);

                DateTime finishNoLaterThanUTC = nowUTC.AddDays(1).Date.AddHours(6).ToUniversalTime();
                if ((finishNoLaterThanUTC - nowUTC).TotalHours < 6) finishNoLaterThanUTC = finishNoLaterThanUTC.AddDays(1);




                var prognoses = await _greenergyAPI.OptimalFutureConsumptionTime(
                    consumptionMinutes: 120,
                    "DK1",
                    DateTime.UtcNow,
                    DateTime.MaxValue
                );



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
