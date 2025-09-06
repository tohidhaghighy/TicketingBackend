using MediatR;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.ChangeDevelopedBy
{
    public class DevelopedByQuery : IRequest<object>
    {
        public string Time { get; set; }
        public Ticketing.Domain.Enums.Developer DeveloperId { get; set; }
        public int TicketId { get; set; }
    }
}
