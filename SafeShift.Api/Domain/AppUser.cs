using Microsoft.AspNetCore.Identity;

namespace SafeShift.Api.Domain;

// IdentityUser already gives us Id, Email, PasswordHash and PhoneNumber.
// We add the SafeShift-specific fields.
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = "";

    // A simple column, copied into the JWT so the API knows what the user may do.
    public UserRole Role { get; set; } = UserRole.Worker;

    public int? SiteId { get; set; }
    public Site? Site { get; set; }

    public ResponderType ResponderType { get; set; } = ResponderType.None;
    public bool IsOnShift { get; set; }
}
