using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using NLog;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Elmah.Io.NLog;

namespace AspNetCoreElmahUI
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
                builder.AddUserSecrets("AspNetCoreElmahUI-c23d2237a4-eb8832a1-452ac5");
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _elmahAppKey = Configuration["ElmahAppKey"];
            _elmahLogId = Configuration["ElmahLogId"];

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            LogManager.Configuration.Variables["configDir"] = "C:\\git\\damienbod\\AspNetCoreElmah\\Logs";

            foreach (ElmahIoTarget target in LogManager.Configuration.AllTargets.Where(t => t is ElmahIoTarget))
            {
                target.ApiKey = _elmahAppKey;
                target.LogId = _elmahLogId;
            }

            LogManager.ReconfigExistingLoggers();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
