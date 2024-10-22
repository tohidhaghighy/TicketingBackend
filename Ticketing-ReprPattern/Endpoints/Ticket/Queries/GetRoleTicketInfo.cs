using FastEndpoints;
using Ticketing.Domain.Contracts;

namespace Ticketing_ReprPattern.Endpoints.Ticket.Queries;

public record MyRequest(int roleId, int userId);

public class GetProjectTicketInfo(
    ITicketService ticketService,
    IProjectService projectService,
    IStatusService statusService,
    ILogger<GetProjectTicketInfo> _logger):Endpoint<MyRequest>
{
    public override void Configure()
    {
        Get("/api/v1/getRoleTicket/{roleId}/{userId}");
    }

    public override async Task HandleAsync(MyRequest request, CancellationToken c)
    {
        try
        {
            var listtickets = await ticketService.ListAsync(a => a.CurrentRoleId == request.roleId || a.UserId == request.userId);
            await SendAsync(new
            {
                opened = listtickets.Where(a=>a.StatusId ==3).Count(),
                rejected = listtickets.Where(a=>a.StatusId ==4).Count(),
                inserted =listtickets.Where(a=>a.StatusId ==2).Count(),
                done = listtickets.Where(a=>a.StatusId ==1).Count(),
                total = listtickets.Count()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Get Error : GetGroupTicketInfoQuery- Handle " + ex.Message);
        }
        
        await SendAsync(new
        {
            opened = 0,
            rejected = 0,
            inserted =0,
            done = 0,
            total = 0
        });
    }
}