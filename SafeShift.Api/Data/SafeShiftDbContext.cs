using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeShift.Api.Domain;

namespace SafeShift.Api.Data;

// The bridge between C# classes and SQL tables. Each DbSet becomes a table.
// IdentityUserContext adds the Users table (and login tables) from ASP.NET Core Identity.
// We use it instead of IdentityDbContext because Role is a simple column, not a roles table.
public class SafeShiftDbContext(DbContextOptions<SafeShiftDbContext> options)
    : IdentityUserContext<AppUser>(options)
{
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentPhoto> IncidentPhotos => Set<IncidentPhoto>();
    public DbSet<IncidentActivity> IncidentActivities => Set<IncidentActivity>();
    public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
    public DbSet<Dispatch> Dispatches => Set<Dispatch>();
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store every enum as text, e.g. "High" instead of 2.
        configurationBuilder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(32);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(u =>
        {
            u.Property(x => x.FullName).HasMaxLength(100);
            u.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId);
        });

        builder.Entity<Site>(s =>
        {
            s.Property(x => x.Name).HasMaxLength(100);
            s.Property(x => x.Code).HasMaxLength(10);
            s.HasIndex(x => x.Code).IsUnique();
            s.HasOne(x => x.PicUser).WithMany().HasForeignKey(x => x.PicUserId);
            s.HasOne(x => x.BackupPicUser).WithMany().HasForeignKey(x => x.BackupPicUserId);
        });

        builder.Entity<Area>(a =>
        {
            a.Property(x => x.Name).HasMaxLength(100);
            a.HasOne(x => x.Site).WithMany(x => x.Areas).HasForeignKey(x => x.SiteId);
            a.HasOne(x => x.PicUser).WithMany().HasForeignKey(x => x.PicUserId);
        });

        builder.Entity<Incident>(i =>
        {
            i.Property(x => x.ReferenceNo).HasMaxLength(20);
            i.Property(x => x.Title).HasMaxLength(200);
            i.Property(x => x.Description).HasMaxLength(2000);
            i.Property(x => x.Location).HasColumnType("geography");

            i.HasIndex(x => x.ReferenceNo).IsUnique();
            i.HasIndex(x => x.ClientId).IsUnique();   // makes offline retries safe
            i.HasIndex(x => new { x.Status, x.ReportedAt });
            i.HasIndex(x => new { x.SiteId, x.ReportedAt });

            i.HasOne(x => x.ReportedBy).WithMany().HasForeignKey(x => x.ReportedById);
            i.HasOne(x => x.ReviewedBy).WithMany().HasForeignKey(x => x.ReviewedById);
        });

        builder.Entity<IncidentPhoto>(p => p.Property(x => x.BlobUrl).HasMaxLength(500));

        builder.Entity<IncidentActivity>(a =>
        {
            a.Property(x => x.Action).HasMaxLength(100);
            a.Property(x => x.Note).HasMaxLength(1000);
        });

        builder.Entity<CorrectiveAction>(c =>
        {
            c.Property(x => x.Description).HasMaxLength(1000);
            c.HasIndex(x => new { x.Status, x.DueDate });
            c.HasOne(x => x.AssignedTo).WithMany().HasForeignKey(x => x.AssignedToId);
            c.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById);
        });

        builder.Entity<UserLocation>(l =>
        {
            l.HasKey(x => x.UserId);
            l.Property(x => x.Location).HasColumnType("geography");
            // The spatial index is added by hand in the first migration (EF can't create it).
        });

        // SQL Server refuses "multiple cascade paths", so deleting is Restrict by default.
        // Only an incident's own children (photos, activities, actions, dispatches) cascade.
        var domainNamespace = typeof(Incident).Namespace;
        foreach (var fk in builder.Model.GetEntityTypes()
                     .Where(e => e.ClrType.Namespace == domainNamespace)
                     .SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = fk.PrincipalEntityType.ClrType == typeof(Incident)
                ? DeleteBehavior.Cascade
                : DeleteBehavior.Restrict;
        }
    }
}
