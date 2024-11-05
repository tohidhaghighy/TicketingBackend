using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;

namespace Ticketing.EndPoints.Search.Query.GetSearchResult
{
    public class GetSearchResultHandler
    {
        public class Handler(ITicketService ticketService, IProjectService projectService, IStatusService statusService, ILogger<GetRoleTicketListHandler> _logger) : IRequestHandler<GetSearchResultQuery, object>
        {
            public async Task<object> Handle(GetSearchResultQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = new List<Domain.Entities.Ticket>();
                    var liststatus = await statusService.ListAsync(null);
                    var listProject = await projectService.ListAsync(null);
                    request.EndDateTime = request.EndDateTime.AddHours(23);// Explanations in the bottom line
                    request.EndDateTime = request.EndDateTime.AddMinutes(59); // to set the end day time to 11:59 p.m
                    result = await ticketService.ListAsync(a =>
                                                          (request.TicketNumber==null || a.TicketNumber == request.TicketNumber) &&
                                                          (request.Title == null || a.Title.Contains(request.Title)) &&
                                                          (request.InsertedRoleId == (int)Role.all || a.InsertedRoleId == request.InsertedRoleId) &&
                                                          (request.Username == null || a.Username.Contains(request.Username)) &&
                                                          (request.CurrentRoleId == (int)Role.all || a.CurrentRoleId == request.CurrentRoleId) &&
                                                          (request.StatusId == (int)StatusId.all || a.StatusId == request.StatusId) &&
                                                          (request.ProjectId == (int)ProjectId.all || a.ProjectId == request.ProjectId) &&
                                                          (request.RequestType == RequestType.all || a.RequestTypeId == request.RequestType) &&
                                                          (request.DeveloperId == Developer.all || a.DeveloperId == request.DeveloperId) &&
                                                          (a.InsertDate >= request.StartDateTime || a.CloseDate >= request.StartDateTime) &&
                                                          (a.InsertDate <= request.EndDateTime || a.CloseDate <= request.EndDateTime));

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
                        InsertedRoleId = x.InsertedRoleId,//new 
                        CurrentRoleId = x.CurrentRoleId,
                        RequestType = x.RequestTypeId,
                        TicketTime = x.TicketTime ?? "0",
                        DeveloperId = x.DeveloperId != 0 ? x.DeveloperId : Developer.unknown,
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
}
