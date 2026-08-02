using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using ProjectFootballSim.GamePersistence.Application;
using ProjectFootballSim.GamePersistence.Infrastructure;
using ProjectFootballSim.Identities.Application;
using ProjectFootballSim.Identities.Infrastructure;
using ProjectFootballSim.Locations.Application;
using ProjectFootballSim.Locations.Infrastructure;
using ProjectFootballSim.Matches.Application;
using ProjectFootballSim.Seasons.Application;
using ProjectFootballSim.Seasons.Infrastructure;
using ProjectFootballSim.Teams.Application;
using ProjectFootballSim.Teams.Infrastructure;
using System.Text;

namespace ProjectFootballSim.Api.Configuration;

internal static class ServicesRegistrator
{
    public static void AddModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentitiesInfrastructure(builder.Configuration, "IdentitiesAzureSql");
        builder.Services.AddIdentitiesApplication(builder.Configuration);

        builder.Services.AddGamePersistenceInfrastructure(builder.Configuration, "GamePersistenceAzureSql");
        builder.Services.AddGamePersistenceApplication();

        builder.Services.AddLocationsInfrastructure(builder.Configuration, "LocationsAzureSql");
        builder.Services.AddLocationsApplication();

        builder.Services.AddTeamsInfrastructure(builder.Configuration, "TeamsAzureSql");
        builder.Services.AddTeamsApplication();

        builder.Services.AddSeasonsInfrastructure(builder.Configuration, "SeasonsAzureSql");
        builder.Services.AddSeasonsApplication();

        builder.Services.AddMatchesApplication();
    }

    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectFootballSim",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectFootballSim",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
            options.AddPolicy("DevFrontend", policy =>
                policy.WithOrigins("http://localhost:5173", "http://localhost:28352")
                      .AllowAnyMethod()
                      .AllowAnyHeader()));

        builder.Services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            };
        });
    }
}
