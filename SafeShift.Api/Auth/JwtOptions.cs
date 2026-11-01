namespace SafeShift.Api.Auth;

// Settings from the "Jwt" section of appsettings.json.
// The Key is a secret: user-secrets / appsettings.Development.json locally,
// App Service settings or Key Vault (Jwt__Key) in Azure.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "SafeShift";
    public string Audience { get; set; } = "SafeShift.Web";
    public string Key { get; set; } = "";
    public int ExpiryMinutes { get; set; } = 480;   // one 8-hour shift
}
