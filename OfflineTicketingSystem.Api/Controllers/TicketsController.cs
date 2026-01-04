using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystem.Api.Data;
using OfflineTicketingSystem.Api.Dtos.Tickets;
using OfflineTicketingSystem.Api.Models;

namespace OfflineTicketingSystem.Api.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TicketsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new InvalidOperationException("User id not found in token.");
        }

        return Guid.Parse(userIdClaim);
    }

    
    // POST /tickets – Create a new ticket (Employee only)
    [HttpPost]
    [Authorize(Policy = "Employee")]
    public async Task<ActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        var userId = GetCurrentUserId();

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
    }

    // GET /tickets/my – List tickets created by the current user (Employee)
    [HttpGet("my")]
    [Authorize(Policy = "Employee")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetMyTickets()
    {
        var userId = GetCurrentUserId();

        var tickets = await _db.Tickets
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(tickets);
    }

    // GET /tickets – List all tickets (Admin only)
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
    {
        var tickets = await _db.Tickets
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(tickets);
    }

    // PUT /tickets/{id} – Update ticket status and assignment (Admin only)
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        ticket.Status = dto.Status;

        if (dto.AssignedToUserId.HasValue)
        {
            var admin = await _db.Users.FirstOrDefaultAsync(u =>
                u.Id == dto.AssignedToUserId.Value && u.Role == UserRole.Admin);

            if (admin == null)
            {
                return BadRequest("AssignedToUserId must be a valid admin user.");
            }

            ticket.AssignedToUserId = dto.AssignedToUserId;
        }

        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // GET /tickets/stats – Show ticket counts by status (Admin only)
    [HttpGet("stats")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<TicketStatsDto>> GetStats()
    {
        var stats = new TicketStatsDto
        {
            Open = await _db.Tickets.CountAsync(t => t.Status == TicketStatus.Open),
            InProgress = await _db.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress),
            Closed = await _db.Tickets.CountAsync(t => t.Status == TicketStatus.Closed)
        };

        return Ok(stats);
    }

    // GET /tickets/{id} – Optional: allowed to creator, assigned admin,(and admins in general)
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Ticket>> GetTicketById(Guid id)
    {
        var ticket = await _db.Tickets
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .SingleOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        //var role = GetCurrentUserRole();
        var role = User.FindFirstValue(ClaimTypes.Role);

        var isCreator = ticket.CreatedByUserId == userId; // creator is allowed
        var isAdmin = role == UserRole.Admin.ToString();
        var isAssignedAdmin = isAdmin && ticket.AssignedToUserId == userId;// Assinged admins are allowed

        if (!isCreator && !isAssignedAdmin)
        {
            return Forbid();
        }

        return Ok(ticket);
    }

    // DELETE /tickets/{id} – Delete a ticket (Admin only)
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> DeleteTicket(Guid id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
