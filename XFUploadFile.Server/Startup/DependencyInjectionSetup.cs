using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace XFUploadFile.Startup;

public static class DependencyInjectionSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddEndpointsApiExplorer();
        services.AddHttpClient();
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSwaggerService(this IServiceCollection services)
    {
        OpenApiSecurityScheme securityScheme = new()
        {
            Name = "Authorization",
            Description = "JWT Authoorization header info using bearer tokens",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        OpenApiSecurityRequirement securityRequirement = new()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                new string[] {}
            }
        };

        services.AddSwaggerGen(opts =>
        {
            string title = "PTA Versioned API";
            string description = "This is a Web API for the new PTA Mobile Device";
            Uri terms = new("https://localhost:7290/terms");
            OpenApiLicense license = new()
            {
                Name = "This is a Boeing proprietary software product"
            };
            OpenApiContact contact = new()
            {
                Name = "Douglas S. Elder PTA Programmer",
                Email = "douglas.s.elder@boeing.com",
                Url = new Uri("https://insite.web.boeing.com/culture/displayBluesInfo.do?bemsId=535547")
            };

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{title} v1",
                Description = description,
                TermsOfService = terms,
                License = license,
                Contact = contact
            });


            opts.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = $"{title} v2",
                Description = description,
                TermsOfService = terms,
                License = license,
                Contact = contact
            });
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            opts.AddSecurityDefinition(name: "bearerAuth", securityScheme: securityScheme);
            opts.AddSecurityRequirement(securityRequirement: securityRequirement);
        });

        return services;
    }

    public static IServiceCollection RegisterVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(opts =>
        {
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.DefaultApiVersion = new(majorVersion: 1, minorVersion: 0);
            opts.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static bool HasTransaction(string jsonTransactionList, int transactionId)
    {
        int[] trans = JsonConvert.DeserializeObject<int[]>(jsonTransactionList);
        return trans.Contains<int>(transactionId);
    }


    public static IServiceCollection RegisterAuthenticationService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(defaultScheme: "Bearer")
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>(key: "Authentication:Issuer"),
                    ValidAudience = configuration.GetValue<string>(key: "Authentication:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        configuration.GetValue<string>(key: "Authentication:SecretKey")))
                };

            });

        return services;

    }

}
