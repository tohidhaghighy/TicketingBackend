using MediatR;
using System.Globalization;
using Ticketing.Application.Service.Stauts;
using Ticketing.Domain.Contracts;
using Ticketing.EndPoints.Ticket.Query.GetGroupTicketInfo;

namespace Ticketing.EndPoints.WorkFlow.Query.GetWorkFlow
{
    public class GetWorkFlowHandler
    {
        public class Handler(ITicketFlowService ticketFlowService, IStatusService statusService , ILogger<GetRoleTicketInfoHandler> _logger) : IRequestHandler<GetWorkFlowQuery, object>
        {
            public async Task<object> Handle(GetWorkFlowQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var ticketFlowList = await ticketFlowService.ListAsync(a => a.TicketId == query.TicketId);
                    var liststatus = await statusService.ListAsync(null);

                    var p = new PersianCalendar();
                    return ticketFlowList.OrderByDescending(a => a.InsertDate).Select(x => new
                    {
                        flowId = x.Id,
                        ticketId = x.TicketId,
                        userId = x.UserId,// Ticket belongs to this user
                        userName = x.Username,// Made by this user name
                        StatusId = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Id,
                        Status = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Name,
                        currentRoleId = x.CurrentRoleId,
                        previousRoleId = x.CurrentRoleId,
                        insertedDate = p.GetHour(x.InsertDate) + ":" +
                                      (p.GetMinute(x.InsertDate) < 10 ? ("0" + p.GetMinute(x.InsertDate)) : p.GetMinute(x.InsertDate)) + " - " +
                                       p.GetYear(x.InsertDate) + "/" +
                                       p.GetMonth(x.InsertDate) + "/" +
                                       p.GetDayOfMonth(x.InsertDate), // 2024/01/01 19:4 -> 2024/01/01 19:04,
                    }).ToList();
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
