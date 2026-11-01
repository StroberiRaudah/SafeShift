using NetTopologySuite.Geometries;

namespace SafeShift.Api.Domain;

// V2: a request sent to the nearest responder ("like Uber").
public class Dispatch
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    public string ResponderId { get; set; } = "";
    public AppUser Responder { get; set; } = null!;
    public double DistanceM { get; set; }
    public DispatchStatus Status { get; set; } = DispatchStatus.Notified;
    public DateTimeOffset NotifiedAt { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public DateTimeOffset? ArrivedAt { get; set; }
}

// V2: the latest known position of an on-shift responder (one row per user).
// Positions older than 2 minutes are ignored when finding the nearest responder.
public class UserLocation
{
    public string UserId { get; set; } = "";
    public AppUser User { get; set; } = null!;
    public Point Location { get; set; } = null!;
    public DateTimeOffset UpdatedAt { get; set; }
}
