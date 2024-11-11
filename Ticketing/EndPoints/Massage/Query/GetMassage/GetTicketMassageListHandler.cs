using System.Globalization;
using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Massage.Query.GetMassage;

public class GetTicketMassageListHandler
{
    public class Handler(IMassageService massageService, ITicketService ticketService,
        IStatusService statusService, IHttpContextAccessor httpContextAccessor, IProjectService projectService, ILogger<GetTicketMassageListQuery> _logger) : IRequestHandler<GetTicketMassageListQuery, object>
    {
        public async Task<object> Handle(GetTicketMassageListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var liststatus = await statusService.ListAsync(null);
                var listprojects = await projectService.ListAsync(null);
                var ticketinfo = await ticketService.GetAsync(a => a.Id == request.TicketId);
                var messageList = await massageService.ListAsync(a => a.TicketId == request.TicketId);
                var p = new PersianCalendar();
                var x = new
                {
                    ticketInfo = new
                    {
                        Id = ticketinfo.Id,
                        TicketNumber = ticketinfo.TicketNumber,
                        Title = ticketinfo.Title,
                        StatusId = ticketinfo.StatusId,
                        Status = liststatus.FirstOrDefault(a => a.Id == ticketinfo.StatusId).Name,
                        Username = ticketinfo.Username,
                        Date = p.GetHour(ticketinfo.InsertDate) + ":" + (p.GetMinute(ticketinfo.InsertDate)<10 ? ("0"+p.GetMinute(ticketinfo.InsertDate)) : p.GetMinute(ticketinfo.InsertDate)) + " - " + p.GetYear(ticketinfo.InsertDate) + "/" + p.GetMonth(ticketinfo.InsertDate) + "/" + p.GetDayOfMonth(ticketinfo.InsertDate), // 2024/01/01 19:4 -> 2024/01/01 19:04
                        Project = listprojects.FirstOrDefault(a => a.Id == ticketinfo.ProjectId).Name,
                        ProjectId = listprojects.FirstOrDefault(a => a.Id == ticketinfo.ProjectId).Id,
                        Priority = ticketinfo.Priority,
                        Text = ticketinfo.Text,
                        File = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" + "/Files/" + ticketinfo.FilePath,
                        UserId = ticketinfo.UserId,
                        HaveFile = !(ticketinfo.FilePath.Trim() == ""),
                        TicketTime=ticketinfo.TicketTime,
                        DeveloperId=ticketinfo.DeveloperId,
                    },
                    messageList = messageList.Select(x => new
                    {
                        Id = x.Id,
                        Text = x.Text,
                        Username = x.Username,
                        UserId = x.UserId,
                        Date = p.GetHour(x.InsertDate) + ":" + (p.GetMinute(x.InsertDate)<10 ? ("0"+p.GetMinute(x.InsertDate)) : p.GetMinute(x.InsertDate)) + " - " + p.GetYear(x.InsertDate) + "/" + p.GetMonth(x.InsertDate) + "/" + p.GetDayOfMonth(x.InsertDate), // 2024/01/01 19:4 -> 2024/01/01 19:04
                        FilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" + "/Files/" + ticketinfo.FilePath,
                        HaveFile = !string.IsNullOrWhiteSpace(x.FilePath)
                    })
                };
                return x;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
            }

            return null;
        }
    }
}