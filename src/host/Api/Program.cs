using ProjectFootballSim.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddModules();
builder.AddApiServices();

var app = builder.Build();
app.UseCors("DevFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapAllEndpoints();

app.Run();
