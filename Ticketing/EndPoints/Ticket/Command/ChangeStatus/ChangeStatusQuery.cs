using MediatR;

namespace Ticketing.EndPoints.Ticket.Command.ChangeStatus;

public class ChangeStatusQuery:IRequest<object>
{
    public int TicketId { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
}