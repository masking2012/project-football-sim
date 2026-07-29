using ProjectFootballSim.FootballSim.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("DevFrontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:28352")
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.AddAllDependencies();

var app = builder.Build();

app.UseCors("DevFrontend");
app.MapAllEndpoints();

app.Run();
