using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Country.Infrastructure;
using ProjectFootballSim.Match.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("DevFrontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:28352")
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.Services.AddCountryInfrastructure(builder.Configuration, "CountryAzureSql");

builder.Services.AddMatchApplication();

var app = builder.Build();

app.UseCors("DevFrontend");
app.MapMatchEndpoints();

app.Run();
