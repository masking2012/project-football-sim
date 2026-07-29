using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Api.Teams;
using ProjectFootballSim.Location.Infrastructure;
using ProjectFootballSim.Match.Application;
using ProjectFootballSim.Team.Application;
using ProjectFootballSim.Team.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("DevFrontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:28352")
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.Services.AddLocationInfrastructure(builder.Configuration, "LocationAzureSql");
builder.Services.AddTeamInfrastructure(builder.Configuration, "TeamAzureSql");
builder.Services.AddTeamApplication();

builder.Services.AddMatchApplication();
builder.Services.AddScoped<TeamStore>();

var app = builder.Build();

app.UseCors("DevFrontend");
app.MapMatchEndpoints();

app.Run();
