using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Greenergy.Settings;
using NSwag.AspNetCore;
using Greenergy.Emissions.Messaging;
using Greenergy.Emissions.Optimization;

namespace Greenergy.Emissions.API.Server
{
    public class Startup
    {
        private const string ApiVersion = "v0.2";
        private IHostingEnvironment _env;
        private ILogger<Startup> _logger;

        // private ILoggerFactory _loggerFactory;
        private IConfiguration _config { get; }

        public Startup(IConfiguration config, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _config = config;
            _env = env;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoSettings>(ms =>
            {
                ms.ConnectionString
                    = _config.GetSection("MongoSettings:ConnectionString").Value;
                ms.Database
                    = _config.GetSection("MongoSettings:Database").Value;

                _logger.LogInformation(ms.Database);
            });

            services.AddTransient<IEmissionsRepository, EmissionsRepository>();
            services.AddTransient<IPrognosisRepository, PrognosisRepository>();
            services.AddTransient<IEmissionDataContext, EmissionDataContext>();
            services.AddTransient<IConsumptionOptimizer, ConsumptionOptimizer>();
            services.AddTransient<IPrognosisMessageSink, PrognosisMessageSink>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = ApiVersion;
                document.Description = "API to retrieve CO2 emissions data related to electricity production.";
                document.Title = "Greenergy Emissions Data API";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUi3();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                _logger.LogInformation("Development Environment");
            }
            else
            {
                app.UseHsts();
                _logger.LogInformation($"Environment: {_env.EnvironmentName}");
            }

            //            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
