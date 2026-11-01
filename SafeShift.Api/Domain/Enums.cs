namespace SafeShift.Api.Domain;

// All enums are stored in the database as strings (see SafeShiftDbContext),
// so the tables stay readable: "High" instead of 2.

public enum UserRole { Worker, Supervisor, HseAdmin }

public enum ResponderType { None, FirstAid, Fire, Maintenance }

public enum IncidentType { Hazard, NearMiss, Incident }

public enum Severity { Low, Medium, High }

public enum IncidentStatus { Submitted, InReview, InfoRequested, ActionOpen, Closed }

public enum ActionStatus { Open, InProgress, Done }

public enum DispatchStatus { Notified, Accepted, Declined, Arrived, Expired }
