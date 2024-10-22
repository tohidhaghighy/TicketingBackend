using Carter;
using MediatR;
using Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketInfo;

public class GetRoleTicketInfoModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/v1/getRoleTicket",
                async (IMediator mediator, [AsParameters] GetRoleTicketInfoQuery query,
                    CancellationToken cancellationToken) =>
                {
                    return await mediator.Send(query, cancellationToken);
                })
            .WithOpenApi()
            .WithTags("Ticket")
            .Produces<object[]>();
    }
}