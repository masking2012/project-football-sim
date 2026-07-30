using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectFootballSim.Identities.Application;
using ProjectFootballSim.Identities.Infrastructure;
using ProjectFootballSim.Locations.Application;
using ProjectFootballSim.Locations.Infrastructure;
using ProjectFootballSim.Matches.Application;
using ProjectFootballSim.Teams.Application;
using ProjectFootballSim.Teams.Infrastructure;

namespace ProjectFootballSim.Api.Configuration;

internal static class DependencyAssembler
{
    public static void AddAllDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentitiesInfrastructure(builder.Configuration, "IdentitiesAzureSql");
        builder.Services.AddIdentitiesApplication();

        builder.Services.AddLocationsInfrastructure(builder.Configuration, "LocationsAzureSql");
        builder.Services.AddLocationsApplication();

        builder.Services.AddTeamsInfrastructure(builder.Configuration, "TeamsAzureSql");
        builder.Services.AddTeamsApplication();

        builder.Services.AddMatchesApplication();

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
    }
}
