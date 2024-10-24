using MediatR;
using Ticketing.Application.Service.Stauts;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;
public class GetRoleTicketListHandler
{
    public class Handler(ITicketService ticketService,IProjectService projectService ,IStatusService statusService,ILogger<GetRoleTicketListHandler> _logger):IRequestHandler<GetRoleTicketListQuery,object>
    {
        public async Task<object> Handle(GetRoleTicketListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new List<Domain.Entities.Ticket>();
                var liststatus = await statusService.ListAsync(null);
                var listProject = await projectService.ListAsync(null);
                if (request.RoleId == (int)Role.adminTaz)
                {
                    result = await ticketService.ListAsync(a => (a.StatusId == request.Status));
                }
                else if (request.RoleId == (int)Role.adminVira)
                {
                    if (request.Status!=2)
                    {
                        result = await ticketService.ListAsync(a => (a.StatusId == request.Status));
                    }
                }
                else
                {
                    if (request.Status == 8)
                    {

                        result = await ticketService.ListAsync(a => (a.CurrentRoleId == request.RoleId &&
                                                                                        (a.StatusId!=1 && a.StatusId != 2 && a.StatusId != 4)) ||
                                                                                        (a.UserId == request.UserId &&
                                                                                        (a.StatusId != 1 && a.StatusId != 2 && a.StatusId != 4)));
                    }
                    else
                    {

                        result = await ticketService.ListAsync(a => (a.CurrentRoleId == request.RoleId &&
                                                                                        a.StatusId == request.Status) ||
                                                                                        (a.UserId == request.UserId &&
                                                                                        a.StatusId == request.Status));

                    }
                }

                var persiandate = new System.Globalization.PersianCalendar();
                return result.OrderByDescending(a=>a.TicketNumber).ToList().Select(x => new
                {
                    Id = x.Id,
                    TicketRowNumber = x.TicketRowNumber,
                    TicketNumber = x.TicketNumber,
                    Title = x.Title,
                    StatusId=liststatus.FirstOrDefault(a=>a.Id==x.StatusId).Id,
                    Status = liststatus.FirstOrDefault(a=>a.Id==x.StatusId).Name,
                    Username= x.Username,
                    Date = persiandate.GetYear(x.InsertDate)+"/"+persiandate.GetMonth(x.InsertDate)+"/"+persiandate.GetDayOfMonth(x.InsertDate),
                    Project = listProject.FirstOrDefault(a=>a.Id==x.ProjectId).Name,
                    Priority= x.Priority,
                    CurrentRoleId= x.CurrentRoleId,
                    RequestType=x.RequestTypeId
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