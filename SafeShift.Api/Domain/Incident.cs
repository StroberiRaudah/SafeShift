using NetTopologySuite.Geometries;

namespace SafeShift.Api.Domain;

// The main table: one row per hazard, near miss or incident report.
public class Incident
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = "";   // e.g. SS-2026-0412

    // Made on the phone before sending. If the phone retries after a dropped
    // connection, the same ClientId must never create a second incident.
    public Guid ClientId { get; set; }

    public IncidentType Type { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public Severity Severity { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Submitted;

    public bool AnyoneHurt { get; set; }
    public bool AreaMadeSafe { get; set; }
    public bool IsAnonymous { get; set; }
    public bool IsLostTimeInjury { get; set; }      // ticked by the supervisor during review

    // GPS point. X = longitude, Y = latitude, SRID 4326.
    public Point? Location { get; set; }
    public double? LocationAccuracyM { get; set; }

    public int SiteId { get; set; }
    public Site Site { get; set; } = null!;
    public int? AreaId { get; set; }
    public Area? Area { get; set; }

    public string ReportedById { get; set; } = "";
    public AppUser ReportedBy { get; set; } = null!;
    public string? ReviewedById { get; set; }
    public AppUser? ReviewedBy { get; set; }

    public DateTimeOffset ReportedAt { get; set; }  // phone time (may be hours before sync)
    public DateTimeOffset SyncedAt { get; set; }    // server time when it arrived

    public int EscalationLevel { get; set; }        // 0 = PIC, 1 = backup PIC, 2 = HSE admin
    public DateTimeOffset? LastEscalatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<IncidentPhoto> Photos { get; set; } = [];
    public List<IncidentActivity> Activities { get; set; } = [];
    public List<CorrectiveAction> CorrectiveActions { get; set; } = [];
}

// The photo file lives in Azure Blob Storage; we only store its URL.
public class IncidentPhoto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    public string BlobUrl { get; set; } = "";
}

// Timeline and audit trail: "Submitted by Aiman", "Escalated to backup PIC", ...
public class IncidentActivity
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    public string? UserId { get; set; }             // null = done by the system
    public AppUser? User { get; set; }
    public string Action { get; set; } = "";
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
