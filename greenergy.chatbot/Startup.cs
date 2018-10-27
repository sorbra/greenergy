using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using greenergy.chatbot_fulfillment.OutputFormatters;
using Greenergy.API;
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
using Swashbuckle.AspNetCore.Swagger;

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
            services.Configure<GreenergyAPISettings>(_config.GetSection("GreenergyAPI"));

            services.AddMvc(config =>
            {
                // Force UTF-8 characterset and remove null elements in Json response . Needed to fix DialogFlow interoperability issue.
                config.OutputFormatters.Insert(0, new DialogFlowJsonOutputFormatter(new JsonSerializerSettings(), ArrayPool<char>.Shared));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IGreenergyAPI, GreenergyAPI>();

            services.AddSwaggerGen(c =>
            {
                // Register the Swagger generator, defining 1 or more Swagger documents
                c.SwaggerDoc(ApiVersion, new Info
                {
                    Version = ApiVersion,
                    Title = "Greenergy Energy Chatbot Backend API",
                    Description = "API to serve Dialogflow chatbot",
                    Contact = new Contact
                    {
                        Name = "Søren Brandt",
                        Email = "sorbra@gmail.com",
                        Url = "https://www.linkedin.com/in/sorenbrandt/"
                    }
                });

                c.CustomSchemaIds((type) => type.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", "Greenergy Energy Data API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                //                app.UseHttpsRedirection();
            }

            app.UseMvc();
        }
    }
}
