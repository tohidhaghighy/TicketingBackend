using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ticketing.EndPoints.Massage.Command.AddMassage;

public class AddMassageModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/v1/addMassage",
                async (IMediator mediator, [FromBody] AddMassageQuery query,
                    CancellationToken cancellationToken) =>
                {
                    return await mediator.Send(query, cancellationToken);
                })
            .WithOpenApi()
            .WithTags("Massage")
            .Produces<object[]>();
    }
}