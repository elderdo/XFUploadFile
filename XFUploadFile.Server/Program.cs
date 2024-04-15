using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PtaWebApi.Startup;
using Serilog;
using XFUploadFile.Startup;

namespace XFUploadFile.Server;

public class Program
{
    public static void Main(string[] args)
    {

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.log")
            .WriteTo.Console()
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
        builder.Services.RegisterServices()
            .RegisterSwaggerService()
            .RegisterVersioningServices()
            .RegisterAuthenticationService(configuration);
        //var loggerFactory = builder.Services.Get
        WebApplication app = builder.Build();

        //Adding Static Files Middleware Component to serve the static files
        IApplicationBuilder _ = app.UseStaticFiles();
        app.ConfigureSwagger();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapControllers();


        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
