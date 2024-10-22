using MediatR;

namespace Ticketing.EndPoints.Ticket.Command.ChangeGroup;

public class ChangeRoleQuery:IRequest<object>
{
    public int TicketId { get; set; }
    public int RoleId { get; set; }
    public int UserId { get; set; }
}