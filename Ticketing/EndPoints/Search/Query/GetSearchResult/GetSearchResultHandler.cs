using DocumentFormat.OpenXml;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Ticketing.Domain.Contracts;
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

                    #region Multi selected items
                    var InsertedRoleId = request.InsertedRoleId.IsNullOrEmpty() ? null : request.InsertedRoleId.ConvertStringToListIntiger();
                    var CurrentRoleId = request.CurrentRoleId.IsNullOrEmpty() ? null : request.CurrentRoleId.ConvertStringToListIntiger();
                    var StatusId = request.StatusId.IsNullOrEmpty() ? null : request.StatusId.ConvertStringToListIntiger();
                    var ProjectId = request.ProjectId.IsNullOrEmpty() ? null : request.ProjectId.ConvertStringToListIntiger();
                    var RequestType = request.RequestType.IsNullOrEmpty() ? null : request.RequestType.ConvertStringToListIntiger();
                    var DeveloperId = request.DeveloperId.IsNullOrEmpty() ? null : request.DeveloperId.ConvertStringToListIntiger();
                    var IsSchadule = request.IsSchadule.IsNullOrEmpty() ? null : request.IsSchadule.ConvertStringToListIntiger();
                    #endregion

                    #region Handel End DateTime
                    request.InsertEndDateTime = request.InsertEndDateTime == DateTime.MinValue || request.InsertEndDateTime == null ? null : request.InsertEndDateTime?.AddHours(23);
                    request.InsertEndDateTime = request.InsertEndDateTime == DateTime.MinValue || request.InsertEndDateTime == null ? null : request.InsertEndDateTime?.AddMinutes(59);

                    request.CloseEndDateTime = request.CloseEndDateTime == DateTime.MinValue || request.CloseEndDateTime == null ? null : request.CloseEndDateTime?.AddHours(23);
                    request.CloseEndDateTime = request.CloseEndDateTime == DateTime.MinValue || request.CloseEndDateTime == null ? null : request.CloseEndDateTime?.AddMinutes(59);
                    #endregion

                    #region TicketNumber logic
                    if (request.TicketNumber.Contains("*"))
                    {
                        request.TicketNumber = request.TicketNumber.Replace("*", "");
                        result = await ticketService.ListAsync(a =>
                                                              (request.TicketNumber.IsNullOrEmpty() || a.TicketNumber.Contains(request.TicketNumber)) &&
                                                              (request.Title.IsNullOrEmpty() || a.Title.Contains(request.Title)) &&
                                                              (request.Username.IsNullOrEmpty() || a.Username.Contains(request.Username)) &&
                                                              (request.InsertedRoleId.IsNullOrEmpty() || InsertedRoleId.Contains(a.InsertedRoleId)) &&
                                                              (request.CurrentRoleId.IsNullOrEmpty() || CurrentRoleId.Contains(a.CurrentRoleId)) &&
                                                              (request.StatusId.IsNullOrEmpty() || StatusId.Contains(a.StatusId)) &&
                                                              (request.ProjectId.IsNullOrEmpty() || ProjectId.Contains(a.ProjectId)) &&
                                                              (request.RequestType.IsNullOrEmpty() || RequestType.Contains((int)a.RequestTypeId)) &&
                                                              (request.DeveloperId.IsNullOrEmpty() || DeveloperId.Contains((int)a.DeveloperId)) &&
                                                              (request.IsSchadule.IsNullOrEmpty() || IsSchadule.Contains((int)a.IsSchedule)) &&
                                                              (!request.InsertStartDateTime.HasValue || a.InsertDate >= request.InsertStartDateTime) &&
                                                              (!request.InsertEndDateTime.HasValue || a.InsertDate <= request.InsertEndDateTime) &&
                                                              (!request.CloseStartDateTime.HasValue || a.ProcessEndDateTime >= request.CloseStartDateTime) &&
                                                              (!request.CloseEndDateTime.HasValue || a.ProcessEndDateTime <= request.CloseEndDateTime));
                    }
                    else
                    {
                        result = await ticketService.ListAsync(a =>
                                                              (request.TicketNumber.IsNullOrEmpty() || a.TicketNumber == request.TicketNumber) &&
                                                              (request.Title.IsNullOrEmpty() || a.Title.Contains(request.Title)) &&
                                                              (request.Username.IsNullOrEmpty() || a.Username.Contains(request.Username)) &&
                                                              (request.InsertedRoleId.IsNullOrEmpty() || InsertedRoleId.Contains(a.InsertedRoleId)) &&
                                                              (request.CurrentRoleId.IsNullOrEmpty() || CurrentRoleId.Contains(a.CurrentRoleId)) &&
                                                              (request.StatusId.IsNullOrEmpty() || StatusId.Contains(a.StatusId)) &&
                                                              (request.ProjectId.IsNullOrEmpty() || ProjectId.Contains(a.ProjectId)) &&
                                                              (request.RequestType.IsNullOrEmpty() || RequestType.Contains((int)a.RequestTypeId)) &&
                                                              (request.DeveloperId.IsNullOrEmpty() || DeveloperId.Contains((int)a.DeveloperId)) &&
                                                              (request.IsSchadule.IsNullOrEmpty() || IsSchadule.Contains((int)a.IsSchedule)) &&
                                                              (!request.InsertStartDateTime.HasValue || a.InsertDate >= request.InsertStartDateTime) &&
                                                              (!request.InsertEndDateTime.HasValue || a.InsertDate <= request.InsertEndDateTime) &&
                                                              (!request.CloseStartDateTime.HasValue || a.ProcessEndDateTime >= request.CloseStartDateTime) &&
                                                              (!request.CloseEndDateTime.HasValue || a.ProcessEndDateTime <= request.CloseEndDateTime));
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
                        ProcessEndDateTime = x.ProcessEndDateTime == null ? "ثبت نشده" : persiandate.GetYear(Convert.ToDateTime(x.ProcessEndDateTime)) + "/" + persiandate.GetMonth(Convert.ToDateTime(x.ProcessEndDateTime)) + "/" + persiandate.GetDayOfMonth(Convert.ToDateTime(x.ProcessEndDateTime)),
                        Project = listProject.FirstOrDefault(a => a.Id == x.ProjectId).Name,
                        Priority = x.Priority,
                        InsertedRoleId = x.InsertedRoleId,//new 
                        CurrentRoleId = x.CurrentRoleId,
                        RequestType = x.RequestTypeId,
                        TicketTime = x.TicketTime ?? "0",
                        DeveloperId = x.DeveloperId,
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
