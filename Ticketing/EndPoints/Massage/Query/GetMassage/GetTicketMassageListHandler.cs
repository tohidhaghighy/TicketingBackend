using System.Globalization;
using MediatR;
using Ticketing.Domain.Contracts;

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
                return new
                {
                    ticketInfo = new
                    {
                        Id = ticketinfo.Id,
                        TicketNumber = ticketinfo.TicketNumber,
                        Title = ticketinfo.Title,
                        StatusId = ticketinfo.StatusId,
                        Status = liststatus.FirstOrDefault(a => a.Id == ticketinfo.StatusId).Name,
                        Username = ticketinfo.Username,
                        Date = p.GetYear(ticketinfo.InsertDate) + "/" + p.GetMonth(ticketinfo.InsertDate) + "/" + p.GetDayOfMonth(ticketinfo.InsertDate),
                        Project = listprojects.FirstOrDefault(a => a.Id == ticketinfo.ProjectId).Name,
                        ProjectId = listprojects.FirstOrDefault(a => a.Id == ticketinfo.ProjectId).Id,
                        Priority = ticketinfo.Priority,
                        Text = ticketinfo.Text,
                        File = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" + "/Files/" + ticketinfo.FilePath,
                        UserId = ticketinfo.UserId,
                        HaveFile = !(ticketinfo.FilePath.Trim() == "")
                    },
                    messageList = messageList.Select(x => new
                    {
                        Id = x.Id,
                        Text = x.Text,
                        Username = x.Username,
                        UserId = x.UserId,
                        Date = p.GetYear(x.InsertDate) + "/" + p.GetMonth(x.InsertDate) + "/" + p.GetDayOfMonth(x.InsertDate),
                        File = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" + "/Files/" + ticketinfo.FilePath,
                        HaveFile = !string.IsNullOrWhiteSpace(x.FilePath)
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
            }

            return null;
        }
    }
}