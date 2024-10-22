using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities;

public class TicketFlow : BaseEntity<int>
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public int TicketId { get; set; }
    public int StatusId { get; set; }
    public int CurrentRoleId { get; set; }
    public int PreviousRoleId { get; set; }
    public DateTime InsertDate { get; set; }
}