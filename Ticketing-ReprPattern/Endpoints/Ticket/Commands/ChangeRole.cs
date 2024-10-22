using FastEndpoints;
using Ticketing_ReprPattern.Endpoints.Ticket.Events;
using Ticketing.Domain.Contracts;

namespace Ticketing_ReprPattern.Endpoints.Ticket.Commands;

public record ChangeRoleRequest(int roleId, int userId,int ticketId);
public class ChangeRole(
    ITicketService ticketService,
    IProjectService projectService,
    IProjectRoleService projectRoleService,
    IStatusService statusService,
    ILogger<ChangeRole> _logger):Endpoint<ChangeRoleRequest>
{
    public override void Configure()
    {
        Post("/api/v1/changeRole");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ChangeRoleRequest request, CancellationToken c)
    {
        try
        {
            var listRoles = await projectRoleService.ListAsync(null);
            var findticket = await ticketService.GetAsync(a => a.Id == request.ticketId);
            await PublishAsync(new CreateTicketFlowEvent()
            {

            });
            if (findticket.CurrentRoleId==5 && request.roleId==4)
            {
                findticket.LastChangeDatetime = DateTime.Now;
            }
            findticket.CurrentRoleId = request.roleId;
            findticket.StatusId = 3;
                
            await SendAsync(await ticketService.UpdateAsync(findticket));
        }
        catch (Exception ex)
        {
            _logger.LogError("Get Error : ChangeGroupHandler- Handle " + ex.Message);
        }

        await SendAsync(null);
    }
}