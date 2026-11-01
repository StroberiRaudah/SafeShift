using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SafeShift.Api.Domain;

namespace SafeShift.Api.Auth;

// The ONLY place that issues login tokens. When the company moves to SSO
// (Microsoft Entra ID), Entra issues the tokens instead and this class goes away.
public class TokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _jwt = options.Value;

    public static SymmetricSecurityKey SigningKey(string key) => new(Encoding.UTF8.GetBytes(key));

    public (string Token, DateTime ExpiresAt) CreateToken(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        // Claims are facts about the user, written inside the token.
        // The API reads them on every request without asking the database.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new("role", user.Role.ToString()),
        };
        if (user.SiteId is int siteId)
            claims.Add(new Claim("site_id", siteId.ToString()));

        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = expiresAt,
            // The signature proves the token came from us and was not changed.
            SigningCredentials = new SigningCredentials(SigningKey(_jwt.Key), SecurityAlgorithms.HmacSha256),
        });

        return (token, expiresAt);
    }
}
