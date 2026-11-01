using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShift.Api.Domain;

namespace SafeShift.Api.Data;

// Fills an empty database with demo sites and users. Runs only once:
// if any user already exists, it does nothing.
public static class DbSeeder
{
    public const string DemoPassword = "Demo@123";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SafeShiftDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        if (await db.Users.AnyAsync()) return;

        var platformA = new Site { Name = "Platform A", Code = "PLA" };
        var kemaman = new Site { Name = "Kemaman Base", Code = "KMB" };
        platformA.Areas.AddRange(
        [
            new Area { Name = "Deck 1 · Process" },
            new Area { Name = "Deck 2 · Pumps" },
            new Area { Name = "Accommodation" },
            new Area { Name = "Wet floor zone", IsHazardZone = true },
        ]);
        kemaman.Areas.Add(new Area { Name = "Warehouse" });
        db.Sites.AddRange(platformA, kemaman);
        await db.SaveChangesAsync();

        await CreateUser(users, "worker@safeshift.demo", "Aiman Hakim",
            UserRole.Worker, platformA, "+60123456701");
        var supervisor = await CreateUser(users, "supervisor@safeshift.demo", "Farid Rahman",
            UserRole.Supervisor, platformA, "+60123456702");
        var admin = await CreateUser(users, "admin@safeshift.demo", "Nurul Aina",
            UserRole.HseAdmin, platformA, "+60123456703");
        await CreateUser(users, "firstaid@safeshift.demo", "Faiz Ismail",
            UserRole.Worker, platformA, "+60123456704", ResponderType.FirstAid, isOnShift: true);

        platformA.PicUserId = supervisor.Id;
        platformA.BackupPicUserId = admin.Id;
        kemaman.PicUserId = supervisor.Id;
        kemaman.BackupPicUserId = admin.Id;
        await db.SaveChangesAsync();
    }

    private static async Task<AppUser> CreateUser(UserManager<AppUser> users, string email,
        string fullName, UserRole role, Site site, string phone,
        ResponderType responderType = ResponderType.None, bool isOnShift = false)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            Role = role,
            SiteId = site.Id,
            PhoneNumber = phone,
            ResponderType = responderType,
            IsOnShift = isOnShift,
        };

        var result = await users.CreateAsync(user, DemoPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Could not create {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }
}
