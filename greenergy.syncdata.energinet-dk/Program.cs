using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Greenergy.Models;
using Greenergy.Database;
using Greenergy.Services;

namespace Greenergy
{
    class Program
    {
        private const string _prefix = "GREENERGY_";
        private const string _appsettings = "appsettings.json";
        private const string _hostsettings = "hostsettings.json";

        public static async Task Main(string[] args)
        {
            // IConfiguration config = new ConfigurationBuilder()
            //     .AddJsonFile("appsettings.json", true, true)
            //     .AddEnvironmentVariables()
            //     .AddCommandLine(args)
            //     .Build();

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile(_hostsettings, optional: true);
                    configHost.AddEnvironmentVariables(prefix: _prefix);
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile(_appsettings, optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables(prefix: _prefix);
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.Configure<Application>(hostContext.Configuration.GetSection("application"));

                    // Add framework services.
                    services.Configure<MongoSettings>(ms =>
                    {
                        ms.ConnectionString
                            = hostContext.Configuration.GetSection("MongoSettings:ConnectionString").Value;
                        ms.Database
                            = hostContext.Configuration.GetSection("MongoSettings:Database").Value;
                    });
                    services.AddTransient<IEmissionsRepository, MongoEmissionsRepository>();

                    services.AddHostedService<GreenergyService>();

                })
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
