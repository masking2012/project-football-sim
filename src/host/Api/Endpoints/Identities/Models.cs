namespace ProjectFootballSim.Api.Endpoints.Identities;

internal sealed record RegisterRequest(string Username, string Password);

internal sealed record LoginRequest(string Username, string Password);

internal sealed record TokenResponse(string Token);
