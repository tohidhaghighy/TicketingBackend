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
                var inProgresCount = 0;

                if (request.RoleId == 4)
                {
                    listtickets = await ticketService.ListAsync(a => (int)a.RequestTypeId == request.RequestTypeId);
                    inProgresCount = listtickets.Where(a => a.StatusId == 8).Count();
                }
                else if (request.RoleId == 5)
                {
                    listtickets = await ticketService.ListAsync(a => (a.StatusId != 2 || a.UserId == request.UserId) && (int)a.RequestTypeId == request.RequestTypeId);
                    inProgresCount = listtickets.Where(a => a.StatusId == 8).Count();
                }
                else
                {
                    listtickets = await ticketService.ListAsync(a => (a.UserId == request.UserId || a.CurrentRoleId == request.RoleId) && (int)a.RequestTypeId == request.RequestTypeId);
                    inProgresCount = listtickets.Where(a => a.StatusId != 1 && a.StatusId != 2 && a.StatusId != 4).Count();
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
                    inProgress = inProgresCount,
                    awaitingRejecting = listtickets.Where(a => a.StatusId == 9).Count(),
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