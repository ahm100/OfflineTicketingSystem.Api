using OfflineTicketingSystem.Api.Models;

namespace OfflineTicketingSystem.Api.Dtos.Tickets;

public class UpdateTicketDto
{
    public TicketStatus Status { get; set; }

    //public TicketPriority? Priority { get; set; }

    // Must be an Admin user if provided
    public Guid? AssignedToUserId { get; set; }
}
