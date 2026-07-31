namespace ProjectFootballSim.Identities.Domain.Entities;

public sealed class User
{
    public Guid Id { get; }
    public string Username { get; }
    public string PasswordHash { get; private set; }

    public User(Guid id, string username, string passwordHash)
    {
        ValidateUsername(username);
        ValidatePassword(passwordHash);

        Id = id;
        Username = username;
        PasswordHash = passwordHash;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        ValidatePassword(newPasswordHash);
        PasswordHash = newPasswordHash;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));
    }
}
