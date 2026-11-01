namespace SafeShift.Api.Domain;

// A work site, e.g. Platform A (PLA) or Kemaman Base (KMB).
public class Site
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";

    // The person in charge gets notified about new reports.
    public string? PicUserId { get; set; }
    public AppUser? PicUser { get; set; }

    // Gets the report if the PIC does not act within 15 minutes.
    public string? BackupPicUserId { get; set; }
    public AppUser? BackupPicUser { get; set; }

    public List<Area> Areas { get; set; } = [];
}

// A part of a site, e.g. "Deck 2 · Pumps". An area PIC overrides the site PIC.
public class Area
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public Site Site { get; set; } = null!;
    public string Name { get; set; } = "";
    public bool IsHazardZone { get; set; }

    public string? PicUserId { get; set; }
    public AppUser? PicUser { get; set; }
}
