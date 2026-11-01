using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SafeShift.Api.Domain;

namespace SafeShift.Api.Auth;

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);

public record UserDto(string Id, string FullName, string Email, string Role, int? SiteId, string? SiteName);

public record LoginResponse(string Token, DateTime ExpiresAt, UserDto User);

[ApiController]
[Route("api/auth")]
public class AuthController(UserManager<AppUser> users, TokenService tokens) : ControllerBase
{
    // POST /api/auth/login  { "email": "...", "password": "..." }
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        // await = "wait for the database without blocking the server".
        var user = await users.Users.Include(u => u.Site)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant());

        // Same message for "no such user" and "wrong password", so attackers
        // can't find out which emails exist.
        if (user is null || !await users.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Wrong email or password." });

        var (token, expiresAt) = tokens.CreateToken(user);
        return new LoginResponse(token, expiresAt, ToDto(user));
    }

    // GET /api/auth/me  (needs "Authorization: Bearer <token>")
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var user = await users.Users.Include(u => u.Site).FirstOrDefaultAsync(u => u.Id == userId);
        return user is null ? Unauthorized() : ToDto(user);
    }

    private static UserDto ToDto(AppUser u) =>
        new(u.Id, u.FullName, u.Email ?? "", u.Role.ToString(), u.SiteId, u.Site?.Name);
}
