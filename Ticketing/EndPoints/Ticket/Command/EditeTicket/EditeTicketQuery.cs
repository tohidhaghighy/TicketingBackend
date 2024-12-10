using MediatR;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.EditeTicket;

public class EditeTicketQuery : IRequest<object>
{
    public string Title { get; set; }
    public string Text { get; set; }
    public Priority Priority { get; set; }
    public RequestType RequestType { get; set; }
    public int ProjectId { get; set; }
    public IsSchedule IsSchedule { get; set; }
    public int TicketId { get; set; }
}
