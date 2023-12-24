namespace TaskManagementSystem.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = "";
    public string ValidAudience { get; set; } = "";
    public string ValidIssuer { get; set; } = "";
}