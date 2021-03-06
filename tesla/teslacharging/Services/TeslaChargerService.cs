using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Greenergy.TeslaCharger.Registry;
using Greenergy.TeslaCharger.Settings;
using Greenergy.TeslaCharger.Constraints;
using Greenergy.TeslaTools;
using Confluent.Kafka;

namespace Greenergy.TeslaCharger.Service
{
    public class TeslaChargerService : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly ITeslaVehiclesRepository _vehicles;
        private readonly IApplicationLifetime _applicationLifetime;

        //        private Timer _chargingCheckTimer;
        private CancellationTokenSource _cts;

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

            _cts = new CancellationTokenSource();

            _logger.LogDebug("Service started: " + _config.Value.Name);

            //            _chargingCheckTimer = new Timer(CheckCharging, null, TimeSpan.Zero, TimeSpan.FromSeconds(_config.Value.ChargingCheckRateInMinutes * 60));

            Task.Run(() => KafkaConsume());

            return Task.CompletedTask;
        }

        private async Task KafkaConsume()
        {
            _logger.LogDebug($"KafkaConsume is running");

            var config = new ConsumerConfig
            {
                GroupId = "teslacharger",
                BootstrapServers = "green-kafka:9092",
                AutoOffsetReset = AutoOffsetResetType.Earliest,
                EnableAutoCommit = true
            };

            using (var c = new Consumer<string, string>(config))
            {
                c.Subscribe("future-consumption");

                c.OnError += (_, e)
                    => Console.WriteLine($"Error: {e.Reason}");
                
                var ct = _cts.Token;

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var cr = c.Consume(ct);
                        Console.WriteLine($"Consumed message from '{cr.Topic}', partion {cr.Partition}, offset {cr.Offset}, length {cr.Value.Length}, head {cr.Value.Substring(0, 30)}");
                        await CheckCharging();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,e.Message);
                    }
                }
            }

            _logger.LogDebug($"KafkaConsume is terminating");
        }
        private async Task CheckCharging()
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
            return Task.CompletedTask;
        }
        private void OnStarted()
        {
            // Perform post-startup activities here
        }
        private void OnStopping()
        {
            _logger.LogDebug("OnStopping");

            _cts.Cancel();
            // Perform on-stopping activities here
        }
        private void OnStopped()
        {
            // Perform post-stopped activities here
        }
        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}
