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
using Swashbuckle.AspNetCore.Swagger;
using Greenergy.Settings;

namespace greenergy.api.server
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
            _logger.LogInformation("ConfigureServices start");

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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
                {
                    // Register the Swagger generator, defining 1 or more Swagger documents
                    c.SwaggerDoc(ApiVersion, new Info
                    {
                        Version = ApiVersion,
                        Title = "Greenergy Energy Data API",
                        Description = "API to retrieve energy consumption related data.",
                        Contact = new Contact
                        {
                            Name = "Søren Brandt",
                            Email = "sorbra@gmail.com",
                            Url = "https://www.linkedin.com/in/sorenbrandt/"
                        }
                    });
                });

            _logger.LogInformation("ConfigureServices end");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", "Greenergy Energy Data API");
            });

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
