using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using greenergy.chatbot_fulfillment.OutputFormatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace greenergy.chatbot_fulfillment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => {
                // Force UTF-8 characterset and remove null elements in Json response . Needed to fix DialogFlow interoperability issue.
                config.OutputFormatters.Insert(0, new DialogFlowJsonOutputFormatter(new JsonSerializerSettings(), ArrayPool<char>.Shared));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
//            .AddJsonOptions(options => {
//                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
//            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc();
        }
    }
}
