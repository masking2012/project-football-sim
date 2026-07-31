namespace ProjectFootballSim.Identities.Infrastructure.Configuration;

public class JwtOptions
{
    public static string SectionName => "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ProjectFootballSim";
    public string Audience { get; set; } = "ProjectFootballSim";
    public int ExpiryMinutes { get; set; } = 60;
}
