using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketInfo;

public class GetRoleTicketInfoHandler
{
    public class Handler(ITicketService ticketService, IProjectService projectService, IStatusService statusService, ILogger<GetRoleTicketInfoHandler> _logger) : IRequestHandler<GetRoleTicketInfoQuery, object>
    {
        public async Task<object> Handle(GetRoleTicketInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var listtickets = await ticketService.ListAsync(a => a.CurrentRoleId == request.RoleId || a.UserId == request.UserId);
                if (request.RoleId == 4)
                {
                    listtickets = await ticketService.ListAsync(null);
                }
                else if (request.RoleId == 5)
                {
                    listtickets = await ticketService.ListAsync(a => a.StatusId != 2 || a.UserId == request.UserId);
                }
                return new
                {
                    done = listtickets.Where(a => a.StatusId == 1).Count(),
                    inserted = listtickets.Where(a => a.StatusId == 2).Count(),
                    sendtovira = listtickets.Where(a => a.StatusId == 3).Count(),
                    rejected = listtickets.Where(a => a.StatusId == 4).Count(),
                    sendtotaz = listtickets.Where(a => a.StatusId == 5).Count(),
                    awaitingConfirmation = listtickets.Where(a => a.StatusId == 6).Count(),
                    inLine = listtickets.Where(a => a.StatusId == 7).Count(),
                    inProgress = listtickets.Where(a => a.StatusId == 8).Count(),
                    total = listtickets.Count()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetGroupTicketInfoQuery- Handle " + ex.Message);
            }

            return null;
        }
    }
}