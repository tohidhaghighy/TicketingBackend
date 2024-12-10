using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticketing.EndPoints.Ticket.Command.EditeTicket;


namespace Ticketing.EndPoints.Ticket.Command;

public class EditeTicketModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1/EditeTicket",
                async (IMediator mediator, [FromBody] EditeTicketQuery query,
                    CancellationToken cancellationToken) =>
                {
                    return await mediator.Send(query, cancellationToken);
                })
            .WithOpenApi()
            .WithTags("Ticket")
            .Produces<object[]>();
    }
}
