using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Modules.Hosting;
using Microsoft.AspNetCore.Mvc.Modules;
using Microsoft.AspNetCore.Mvc.Modules.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchard.DisplayManagement;
using Orchard.Environment.Commands;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Shell.Data;

namespace Orchard.Cms.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"logging.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThemingHost();
            services.AddThemeFolder("Themes");
            services.AddSitesFolder("App_Data", "Sites");
            services.AddCommands();

            services.AddModuleServices(configure => configure
                .AddMvcModules()
                .AddConfiguration(Configuration)
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            loggerFactory.AddConsole(Configuration);

            app.UseModules(modules => modules
                .UseStaticFilesModules()
                .UseMvcModules()
            );
        }
    }
}