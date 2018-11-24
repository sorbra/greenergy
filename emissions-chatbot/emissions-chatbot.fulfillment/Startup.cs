using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using greenergy.chatbot_fulfillment;
using greenergy.chatbot_fulfillment.OutputFormatters;
using Greenergy.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Greenergy.Emissions.API;

namespace greenergy.chatbot_fulfillment
{
    public class Startup
    {
        private const string ApiVersion = "v0.1";

        public IConfiguration _config { get; set; }

        private ILogger<Startup> _logger;

        public Startup(IConfiguration config, ILogger<Startup> logger)
        {
            _config = config;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FulfillmentSettings>(_config.GetSection("Fulfillment"));

            services.AddMvc(config =>
            {
                // Force UTF-8 characterset and remove null elements in Json response . Needed to fix DialogFlow interoperability issue.
                config.OutputFormatters.Insert(0, new DialogFlowJsonOutputFormatter(new JsonSerializerSettings(), ArrayPool<char>.Shared));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                //                app.UseHttpsRedirection();
            }

            foreach (var conf in _config.GetSection("Fulfillment").AsEnumerable().ToList())
            {
                _logger.LogInformation($"{conf.Key}, {conf.Value}");
            }
            foreach (var conf in _config.GetSection("GreenergyAPI").AsEnumerable().ToList())
            {
                _logger.LogInformation($"{conf.Key}, {conf.Value}");
            }

            app.UseMvc();
        }
    }
}
