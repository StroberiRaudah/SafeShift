namespace SafeShift.Api.Domain;

// A fix assigned by a supervisor, e.g. "Lay anti-slip mats at the wash bay".
public class CorrectiveAction
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    public string Description { get; set; } = "";

    public string AssignedToId { get; set; } = "";
    public AppUser AssignedTo { get; set; } = null!;
    public string CreatedById { get; set; } = "";
    public AppUser CreatedBy { get; set; } = null!;

    public DateOnly DueDate { get; set; }
    public ActionStatus Status { get; set; } = ActionStatus.Open;
    public DateTimeOffset? CompletedAt { get; set; }

    // "Overdue" is calculated, never stored, so it can never be out of date.
    public bool IsOverdue(DateOnly today) => Status != ActionStatus.Done && today > DueDate;
}
