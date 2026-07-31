using System.ComponentModel.DataAnnotations;

namespace ProjectFootballSim.Identities.Infrastructure.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public required string Secret { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    public int ExpiryMinutes { get; init; } = 60;
}
