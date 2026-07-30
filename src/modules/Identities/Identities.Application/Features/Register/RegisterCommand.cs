using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Identities.Domain.Entities;
using ProjectFootballSim.Identities.Infrastructure.Database;

namespace ProjectFootballSim.Identities.Application.Features.Register;

public sealed class RegisterCommand(IdentitiesDbContext dbContext, IPasswordHasher<AppUser> passwordHasher)
{
    public async Task<RegisterResult> HandleAsync(string username, string password, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
            return RegisterResult.UsernameTaken;

        var placeholder = new AppUser(Guid.NewGuid(), username, string.Empty);
        var hash = passwordHasher.HashPassword(placeholder, password);
        var user = new AppUser(placeholder.Id, username, hash);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return RegisterResult.Success;
    }
}

public enum RegisterResult
{
    Success,
    UsernameTaken,
}
