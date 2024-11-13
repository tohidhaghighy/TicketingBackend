using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Command.ChangeStatus;

public class ChangeStatusHandler
{
    public class Handler(ITicketService ticketService,ITicketFlowService ticketFlowService,ILogger<ChangeStatusHandler> _logger):IRequestHandler<ChangeStatusQuery,object>
    {
        public async Task<object> Handle(ChangeStatusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var findticket = await ticketService.GetAsync(a => a.Id == request.TicketId);
                findticket.StatusId = request.Status;
                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = findticket.InsertedRoleId,
                    InsertDate = DateTime.Now,
                    StatusId = findticket.StatusId,
                    Username = findticket.Username,
                    UserId = request.UserId,
                    TicketId = findticket.Id,
                    PreviousRoleId = findticket.CurrentRoleId
                });
                return await ticketService.UpdateAsync(findticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : ChangeStatusHandler- Handle " + ex.Message);
            }

            return null;
        }
    }
}