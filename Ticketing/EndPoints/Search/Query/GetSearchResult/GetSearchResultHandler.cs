using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;
using Ticketing.Utility;

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

                    #region TicketNumber logic
                    if (request.TicketNumber.Contains("*"))
                    {
                        request.TicketNumber = request.TicketNumber.Replace("*", "");
                        result = await ticketService.ListAsync(a =>
                                                              (request.TicketNumber == "" || a.TicketNumber.Contains(request.TicketNumber)));
                    }
                    else
                    {
                        result = await ticketService.ListAsync(a =>
                                                              (request.TicketNumber == "" || a.TicketNumber == request.TicketNumber));
                    }
                    #endregion

                    #region Multiple select filters
                    if(request.Title != "")
                    {
                        result = result.Where(a => a.Title.Contains(request.Title)).ToList();
                    }
                    if(request.Username != "")
                    {
                        result = result.Where(a => a.Username.Contains(request.Username)).ToList();
                    }

                    var InsertedRoleId = request.InsertedRoleId.ConvertStringToListIntiger();
                    if (request.InsertedRoleId != "")
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach (var roleId in request.InsertedRoleId)
                        {
                            temResult.AddRange(result.Where(a => a.InsertedRoleId == roleId));
                        }
                        result = temResult.Distinct().ToList();
                    }
                    if (request.CurrentRoleId.Contains((int)Role.all))
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach(var currentRoleId in request.CurrentRoleId)
                        {
                            temResult.AddRange(result.Where(a => a.CurrentRoleId == currentRoleId));
                        }
                        result = temResult.Distinct().ToList();
                    }
                    if(request.StatusId.Contains((int)StatusId.all))
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach(var statusId in request.StatusId)
                        {
                            temResult.AddRange(result.Where(a => a.StatusId == statusId));
                        }
                        result = temResult.Distinct().ToList();
                    }
                    if(request.ProjectId.Contains((int)ProjectId.all))
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach( var projectId in request.ProjectId)
                        {
                            temResult.AddRange(result.Where(a => a.ProjectId == projectId));
                        }
                        result = temResult.Distinct().ToList();
                    }
                    if(request.RequestType.Contains((int)RequestType.all))
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach(var requestTypeId in request.RequestType)
                        {
                            temResult.AddRange(result.Where(a => (int)a.RequestTypeId == requestTypeId));
                        }
                        result = temResult.Distinct().ToList();
                    }
                    if(request.DeveloperId.Contains((int)Developer.all))
                    {
                        var temResult = new List<Domain.Entities.Ticket>();
                        foreach(var developerId in request.DeveloperId)
                        {
                            temResult.AddRange(result.Where(a => (int)a.DeveloperId == developerId));
                        }
                        result = temResult.Distinct().ToList(); //ensures no duplicate records exist in the filtered result
                    }

                    //result = result.Where(a =>
                    //                     (request.Title == "" || a.Title.Contains(request.Title)) &&
                    //                     (request.InsertedRoleId.Contains((int)Role.all) || request.InsertedRoleId.Contains(a.InsertedRoleId) &&
                    //                     (request.Username == "" || a.Username.Contains(request.Username)) &&
                    //                     (request.CurrentRoleId.Contains((int)Role.all) || request.CurrentRoleId.Contains(a.CurrentRoleId)) &&
                    //                     (request.StatusId.Contains((int)StatusId.all) || request.StatusId.Contains(a.StatusId)) &&
                    //                     (request.ProjectId.Contains((int)ProjectId.all) || request.ProjectId.Contains(a.ProjectId)) &&
                    //                     (request.RequestType.Contains((int)RequestType.all) || request.RequestType.Contains((int)a.RequestTypeId)) &&
                    //                     (request.DeveloperId.Contains((int)Developer.all) || request.DeveloperId.Contains((int)a.DeveloperId)).ToList();

                    #endregion

                    #region Handel time scope
                    if (request.InsertStartDateTime != null)
                    {
                        result = result.Where(a => (
                                             request.InsertStartDateTime == DateTime.MinValue || a.InsertDate >= request.InsertStartDateTime)).ToList();
                    }
                    if (request.InsertEndDateTime != null)
                    {
                        request.InsertEndDateTime = request.InsertEndDateTime?.AddHours(23);// Explanations in the bottom line
                        request.InsertEndDateTime = request.InsertEndDateTime?.AddMinutes(59); // to set the end day time to 11:59 p.m
                        result = result.Where(a => (
                                             request.InsertEndDateTime == DateTime.MinValue || a.InsertDate <= request.InsertEndDateTime)).ToList();
                    }
                    if (request.CloseStartDateTime != null)
                    {
                        result = result.Where(a => (request.CloseStartDateTime == DateTime.MinValue || a.ProcessEndDateTime <= request.CloseStartDateTime)).ToList();
                    }
                    if(request.CloseEndDateTime != null)
                    {
                        request.CloseEndDateTime = request.CloseEndDateTime?.AddHours(23);
                        request.CloseEndDateTime = request.CloseEndDateTime?.AddMinutes(59);
                        result = result.Where(a => (request.CloseEndDateTime == DateTime.MinValue || a.ProcessEndDateTime <= request.CloseEndDateTime)).ToList();
                    }
                    #endregion

                    #region return structure
                    var persiandate = new System.Globalization.PersianCalendar();
                    return result.OrderByDescending(a => a.TicketRowNumber).ToList().Select(x => new
                    {
                        Id = x.Id,
                        TicketRowNumber = x.TicketRowNumber,
                        TicketNumber = x.TicketNumber,
                        Title = x.Title,
                        StatusId = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Id,
                        Status = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Name,
                        Username = x.Username,
                        InsertDate = persiandate.GetYear(x.InsertDate) + "/" + persiandate.GetMonth(x.InsertDate) + "/" + persiandate.GetDayOfMonth(x.InsertDate),
                        CloseDate = persiandate.GetYear(x.CloseDate) + "/" + persiandate.GetMonth(x.CloseDate) + "/" + persiandate.GetDayOfMonth(x.CloseDate),
                        Project = listProject.FirstOrDefault(a => a.Id == x.ProjectId).Name,
                        Priority = x.Priority,
                        InsertedRoleId = x.InsertedRoleId,//new 
                        CurrentRoleId = x.CurrentRoleId,
                        RequestType = x.RequestTypeId,
                        TicketTime = x.TicketTime ?? "0",
                        DeveloperId = x.DeveloperId != 0 ? x.DeveloperId : Developer.unknown,
                    });
                    #endregion
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
