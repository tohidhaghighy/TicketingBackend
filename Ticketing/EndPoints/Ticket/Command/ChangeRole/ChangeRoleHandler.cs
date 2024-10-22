using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Command.ChangeGroup;

public class ChangeRoleHandler
{
    public class Handler(ITicketService ticketService,IProjectRoleService projectRoleService,ITicketFlowService ticketFlowService,ILogger<ChangeRoleHandler> _logger):IRequestHandler<ChangeRoleQuery,object>
    {
        public async Task<object> Handle(ChangeRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var listRoles = await projectRoleService.ListAsync(null);
                var findticket = await ticketService.GetAsync(a => a.Id == request.TicketId);
                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = request.RoleId,
                    InsertDate = DateTime.Now,
                    StatusId = findticket.StatusId,
                    UserId = request.UserId,
                    Username = findticket.Username,
                    TicketId = findticket.Id,
                    PreviousRoleId = findticket.CurrentRoleId
                });
                if (findticket.CurrentRoleId==5 && request.RoleId==4)
                {
                    findticket.LastChangeDatetime = DateTime.Now;
                    findticket.StatusId = 5;
                }
                else
                {
                    findticket.StatusId = 3;
                }
                findticket.CurrentRoleId = request.RoleId;
                
                return await ticketService.UpdateAsync(findticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : ChangeGroupHandler- Handle " + ex.Message);
            }
            return null;
        }
    }
}