namespace OfflineTicketingSystem.Api.Models;

public enum UserRole
{
    Employee = 0,
    Admin = 1
}

public enum TicketStatus
{
    Open = 0,
    InProgress = 1,
    Closed = 2
}

public enum TicketPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}
