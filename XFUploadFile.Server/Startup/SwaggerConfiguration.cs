using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace PtaWebApi.Startup;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

/// <summary>
/// 
/// </summary>
public static class SwaggerConfiguration
{
    public static WebApplication ConfigureSwagger(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                //opts.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                // opts.RoutePrefix = string.Empty;
            });
        }
        return app;
    }
}
