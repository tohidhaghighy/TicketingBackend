using MediatR;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.ChangeDevelopedBy
{
    public class DevelopedByQuery:IRequest<object>
    {
        public string? TicketTime { get; set; }
        public Developer DeveloperId { get; set; }
        public int TicketId { get; set; }
    }
}
