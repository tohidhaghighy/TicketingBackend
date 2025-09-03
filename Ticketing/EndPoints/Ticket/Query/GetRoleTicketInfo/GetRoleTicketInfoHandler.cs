using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

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

                if (request.RoleId == (int)Role.adminitm)
                {
                    listtickets = await ticketService.ListAsync(a => (int)a.RequestTypeId == request.RequestTypeId);
                }
                else
                {
                    listtickets = await ticketService.ListAsync(a => (a.UserId == request.UserId || a.CurrentRoleId == request.RoleId) && (int)a.RequestTypeId == request.RequestTypeId);
                }

                return new
                {
                    done = listtickets.Where(a => a.StatusId == (int)StatusId.done).Count(),
                    inserted = listtickets.Where(a => a.StatusId == (int)StatusId.inserted).Count(),
                    sendtovira = listtickets.Where(a => a.StatusId == (int)StatusId.sendtovira).Count(),
                    rejected = listtickets.Where(a => a.StatusId == (int)StatusId.rejected).Count(),
                    sendtotaz = listtickets.Where(a => a.StatusId == (int)StatusId.sendtotaz).Count(),
                    awaitingConfirmation = listtickets.Where(a => a.StatusId == (int)StatusId.awaitingConfirmation).Count(),
                    inLine = listtickets.Where(a => a.StatusId == (int)StatusId.inLine).Count(),
                    inProgress = listtickets.Where(a => a.StatusId == (int)StatusId.inProgress).Count(),
                    awaitingRejecting = listtickets.Where(a => a.StatusId == (int)StatusId.awaitingRejecting).Count(),
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