using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Command.ChangeDevelopedBy;
public class DevelopedByHandler
{
    public class Handler(ITicketService ticketService, ILogger<DevelopedByQuery> _logger) : IRequestHandler<DevelopedByQuery, object>
    {
        public async Task<object> Handle(DevelopedByQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var ticketinfo = await ticketService.GetAsync(a => a.Id == request.TicketId);
                ticketinfo.TicketTime = request.Time;
                ticketinfo.DeveloperId = request.DeveloperId;

                var result = await ticketService.UpdateAsync(ticketinfo);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : AddTicketHandler- Handle " + ex.Message);
            }

            return null;
        }
    }
}


