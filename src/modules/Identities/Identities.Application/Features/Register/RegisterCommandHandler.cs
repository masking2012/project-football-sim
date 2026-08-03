using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Identities.Domain.Entities;
using ProjectFootballSim.Identities.Infrastructure.Database;

namespace ProjectFootballSim.Identities.Application.Features.Register;

public sealed class RegisterCommandHandler(IdentitiesDbContext dbContext, IPasswordHasher<User> passwordHasher)
{
    public async Task<RegisterResult> HandleAsync(string username, string password, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
            return RegisterResult.UsernameTaken;

        var user = new User(Guid.NewGuid(), username, password);
        var hash = passwordHasher.HashPassword(user, password);
        user.UpdatePassword(hash);

        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex) when (ex.InnerException is not null &&
            ex.InnerException.Message.Contains("unique", StringComparison.OrdinalIgnoreCase))
        {
            return RegisterResult.UsernameTaken;
        }

        return RegisterResult.Success;
    }
}
