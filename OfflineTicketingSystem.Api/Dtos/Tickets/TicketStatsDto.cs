namespace OfflineTicketingSystem.Api.Dtos.Tickets;

public class TicketStatsDto
{
    public int Open { get; set; }

    public int InProgress { get; set; }

    public int Closed { get; set; }
}
