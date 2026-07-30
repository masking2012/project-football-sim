namespace ProjectFootballSim.Identities.Domain.Entities;

public sealed class AppUser
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public AppUser(Guid id, string username, string passwordHash)
    {
        ValidateUsername(username);

        Id = id;
        Username = username;
        PasswordHash = passwordHash;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
    }
}
