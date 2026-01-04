using OfflineTicketingSystem.Api.Models;

namespace OfflineTicketingSystem.Api.Dtos.Tickets;

public class CreateTicketDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
