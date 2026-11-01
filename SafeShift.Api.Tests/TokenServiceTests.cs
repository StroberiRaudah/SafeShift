using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SafeShift.Api.Auth;
using SafeShift.Api.Domain;

namespace SafeShift.Api.Tests;

public class TokenServiceTests
{
    private static readonly JwtOptions Jwt = new()
    {
        Issuer = "SafeShift",
        Audience = "SafeShift.Web",
        Key = "test-key-that-is-at-least-32-characters-long",
        ExpiryMinutes = 60,
    };

    private static readonly AppUser Supervisor = new()
    {
        Id = "user-1",
        Email = "supervisor@safeshift.demo",
        FullName = "Farid Rahman",
        Role = UserRole.Supervisor,
        SiteId = 1,
    };

    private static TokenValidationParameters ValidationWith(string key) => new()
    {
        ValidIssuer = Jwt.Issuer,
        ValidAudience = Jwt.Audience,
        IssuerSigningKey = TokenService.SigningKey(key),
    };

    [Fact]
    public async Task Token_contains_user_id_role_and_site()
    {
        var (token, _) = new TokenService(Options.Create(Jwt)).CreateToken(Supervisor);

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, ValidationWith(Jwt.Key));

        Assert.True(result.IsValid);
        Assert.Equal("user-1", result.Claims[JwtRegisteredClaimNames.Sub]);
        Assert.Equal("Supervisor", result.Claims["role"]);
        Assert.Equal("1", result.Claims["site_id"]);
    }

    [Fact]
    public void Token_expires_after_configured_minutes()
    {
        var (_, expiresAt) = new TokenService(Options.Create(Jwt)).CreateToken(Supervisor);

        Assert.InRange(expiresAt, DateTime.UtcNow.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
    }

    [Fact]
    public async Task Token_signed_with_another_key_is_rejected()
    {
        var (token, _) = new TokenService(Options.Create(Jwt)).CreateToken(Supervisor);

        var result = await new JsonWebTokenHandler()
            .ValidateTokenAsync(token, ValidationWith("a-different-key-that-is-also-32-chars-long"));

        Assert.False(result.IsValid);
    }
}
