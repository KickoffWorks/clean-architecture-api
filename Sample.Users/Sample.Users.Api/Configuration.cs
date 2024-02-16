using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sample.Core.Domain.Settings;
using Swashbuckle.AspNetCore.Filters;
using Sample.Users.Application.Endpoints;

namespace Sample.Users.Api;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                          ?? builder.Services.BuildServiceProvider()?.GetService<IWebHostEnvironment>()?.EnvironmentName
                          ?? string.Empty;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();
        
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddHealthChecks();

        builder.Services.AddDbContext<Sample.Core.Infrastructure.ApplicationDbContext>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var appSettings = configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>()!;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = appSettings.AuthenticationSettings.Issuer,
                ValidAudiences = appSettings.AuthenticationSettings.Audiences,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(appSettings.AuthenticationSettings.IssuerSigningKey))
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddIdentityCore<Sample.Core.Domain.Entities.ApplicationUser>()
            .AddEntityFrameworkStores<Sample.Core.Infrastructure.ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/api/auth").MapUserEndpoints(); // This can be used for static class endpoints.
    }
}