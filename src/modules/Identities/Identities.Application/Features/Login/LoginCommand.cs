using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Identities.Domain.Entities;
using ProjectFootballSim.Identities.Infrastructure.Database;
using ProjectFootballSim.Identities.Infrastructure.Services;

namespace ProjectFootballSim.Identities.Application.Features.Login;

public sealed class LoginCommand(
    IdentitiesDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    JwtTokenGenerator tokenGenerator)
{
    public async Task<AuthResultDto?> HandleAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return null;

        var token = tokenGenerator.Generate(user);
        return new AuthResultDto(token);
    }
}
