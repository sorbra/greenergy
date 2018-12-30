using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Greenergy.TeslaCharger.Settings;
using NSwag.AspNetCore;
using Greenergy.TeslaCharger.Service;

namespace Greenergy.TeslaCharger.Registry
{
    public class Startup
    {
        private const string ApiVersion = "v0.1";
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
            // services.Configure<MongoSettings>(ms =>
            // {
            //     ms.ConnectionString
            //         = _config.GetSection("Mongo:ConnectionString").Value;
            //     ms.Database
            //         = _config.GetSection("Mongo:Database").Value;

            //     _logger.LogInformation(ms.Database);
            // });
            services.Configure<ApplicationSettings>(_config.GetSection("Application"));
            services.Configure<MongoSettings>(_config.GetSection("Mongo"));

            services.AddTransient<ITeslaVehiclesRepository, TeslaVehiclesRepository>();
            services.AddTransient<ITeslaDataContext, TeslaDataContext>();
            services.AddHostedService<TeslaChargerService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = ApiVersion;
                document.Description = "API to manage information about Teslas with managed charging";
                document.Title = "Greenergy Tesla Charger API";
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
