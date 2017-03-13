using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;

namespace AspNetCoreElmah
{
    public class Startup
    {
        private string _elmahAppKey;
        private string _elmahLogId;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets("AspNetCoreElmah-c23d2237a4-eb8832a1-452ac4");
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _elmahAppKey = Configuration["ElmahAppKey"];
            _elmahLogId = Configuration["ElmahLogId"];
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddElmahIo(
                _elmahAppKey, 
                new Guid(_elmahLogId), 
                new FilterLoggerSettings
                {
                    {"ValuesController", LogLevel.Information}
                },
                new ElmahIoProviderOptions
                {
                    OnMessage = msg =>
                    {
                        msg.Version = "1.0.0";
                        msg.Hostname = "dev";
                        msg.Application = "AspNetCoreElmah";
                    }
                });
            app.UseElmahIo(_elmahAppKey, new Guid(_elmahLogId));

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
