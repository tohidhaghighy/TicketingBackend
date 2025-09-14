using MediatR;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.ChangeDevelopedBy
{
    public class DevelopedByQuery : IRequest<object>
    {
        public string Time { get; set; }
        public int AssignUserId { get; set; }
        public int TicketId { get; set; }
        public string AssignUserName { get; set; }
    }
}
