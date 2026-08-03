using ProjectFootballSim.Identities.Application.Features.Login;
using ProjectFootballSim.Identities.Application.Features.Register;

namespace ProjectFootballSim.Api.Endpoints.Identities;

internal static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register",
            async (RegisterRequest request, RegisterCommandHandler handler, CancellationToken cancellationToken) =>
            {
                RegisterResult result = await handler.HandleAsync(request.Username, request.Password, cancellationToken)
                    .ConfigureAwait(false);

                return result switch
                {
                    RegisterResult.Success => Results.Ok(),
                    RegisterResult.UsernameTaken => Results.Conflict("Username is already taken."),
                    _ => Results.StatusCode(500),
                };
            });

        app.MapPost("/api/auth/login",
            async (LoginRequest request, LoginCommandHandler handler, CancellationToken cancellationToken) =>
            {
                var authResult = await handler.HandleAsync(request.Username, request.Password, cancellationToken)
                    .ConfigureAwait(false);

                return authResult is null
                    ? Results.Unauthorized()
                    : Results.Ok(new TokenResponse(authResult.Token));
            });
    }
}
