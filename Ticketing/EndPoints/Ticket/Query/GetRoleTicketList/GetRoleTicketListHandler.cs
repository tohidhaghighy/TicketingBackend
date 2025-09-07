using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;
public class GetRoleTicketListHandler
{
    public class Handler(ITicketService ticketService, IProjectService projectService, IStatusService statusService, ILogger<GetRoleTicketListHandler> _logger) : IRequestHandler<GetRoleTicketListQuery, object>
    {
        public async Task<object> Handle(GetRoleTicketListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new List<Domain.Entities.Ticket>();
                var liststatus = await statusService.ListAsync(null);
                var listProject = await projectService.ListAsync(null);
                if (request.RoleId == (int)Role.admindir || request.RoleId == (int)Role.TicketingAdmin)
                {
                    result = await ticketService.ListAsync(a => (a.StatusId == request.Status && (int)a.RequestTypeId == request.RequestTypeId));
                }
                else
                {
                    result = await ticketService.ListAsync(a => (a.CurrentRoleId == request.RoleId &&
                                                                                        a.StatusId == request.Status) ||
                                                                                        (a.UserId == request.UserId &&
                                                                                        a.StatusId == request.Status) &&
                                                                                        (int)a.RequestTypeId == request.RequestTypeId);
                }

                var persiandate = new System.Globalization.PersianCalendar();
                return result.OrderByDescending(a => a.TicketNumber).ToList().Select(x => new
                {
                    Id = x.Id,
                    TicketRowNumber = x.TicketRowNumber,
                    TicketNumber = x.TicketNumber,
                    Title = x.Title,
                    StatusId = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Id,
                    Status = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Name,
                    Username = x.Username,
                    Date = persiandate.GetYear(x.InsertDate) + "/" + persiandate.GetMonth(x.InsertDate) + "/" + persiandate.GetDayOfMonth(x.InsertDate),
                    Project = listProject.FirstOrDefault(a => a.Id == x.ProjectId).Name,
                    Priority = x.Priority,
                    InsertedRoleId = x.InsertedRoleId,
                    CurrentRoleId = x.CurrentRoleId,
                    RequestType = x.RequestTypeId,
                    TicketTime = x.TicketTime ?? "0",
                    DeveloperId = x.DeveloperId != Ticketing.Domain.Enums.Developer.all ? x.DeveloperId : Ticketing.Domain.Enums.Developer.unknown,
                    userId = x.UserId,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
            }

            return null;
        }
    }
}