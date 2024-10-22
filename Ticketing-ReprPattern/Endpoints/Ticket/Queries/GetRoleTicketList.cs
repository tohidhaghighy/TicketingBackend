using FastEndpoints;
using Ticketing.Domain.Contracts;

namespace Ticketing_ReprPattern.Endpoints.Ticket.Queries;

public record GetRoleTicketListRequest
{
    public int RoleId { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
}

public class GetRoleTicketList(
    ITicketService ticketService,
    IProjectService projectService,
    IStatusService statusService,
    ILogger<GetProjectTicketInfo> _logger) : Endpoint<GetRoleTicketListRequest>
{
    public override void Configure()
    {
        Get("/api/v1/getTicketList/{roleId}/{status}/{userId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRoleTicketListRequest request, CancellationToken c)
    {
        try
        {
            var liststatus = await statusService.ListAsync(null);
            var listProject = await projectService.ListAsync(null);
            var findstatus = liststatus.FirstOrDefault(a => a.Id == request.Status);
            var result = await ticketService.ListAsync(a => (a.CurrentRoleId == request.RoleId &&
                                                             a.StatusId == findstatus.Id) ||
                                                            a.UserId == request.UserId);
            var persiandate = new System.Globalization.PersianCalendar();
            await SendAsync(result.OrderByDescending(a => a.InsertDate).Select(x => new
            {
                Id = x.Id,
                TicketRowNumber = x.TicketRowNumber,
                TicketNumber = x.TicketNumber,
                Title = x.Title,
                StatusId = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Id,
                Status = liststatus.FirstOrDefault(a => a.Id == x.StatusId).Name,
                Username = x.Username,
                Date = persiandate.GetYear(x.InsertDate) + "/" + persiandate.GetMonth(x.InsertDate) + "/" +
                       persiandate.GetDayOfMonth(x.InsertDate),
                Project = listProject.FirstOrDefault(a => a.Id == x.ProjectId).Name,
                Priority = x.Priority
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
        }

        await SendAsync(null);
    }
}