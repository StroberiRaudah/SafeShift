using SafeShift.Api.Domain;

namespace SafeShift.Api.Incidents;

// What the phone sends when a worker submits a report.
public record CreateIncidentRequest(
    Guid ClientId,              // made on the phone, stops duplicate reports eg 3f2b8c1e-9a4d-4e7b-b6f1-2c8d0e5a7b93
    IncidentType Type,          // Hazard / NearMiss / Incident
    string Title,
    string? Description,        // optional, so it has ?
    Severity Severity,               // blank 1: which enum? Look in Enums.cs
    bool AnyoneHurt,
    bool AreaMadeSafe,           // blank 2: yes/no
    bool IsAnonymous,
    DateTimeOffset ReportedAt          // blank 3: when it happened on the phone. Look in Incident.cs line 40
);
