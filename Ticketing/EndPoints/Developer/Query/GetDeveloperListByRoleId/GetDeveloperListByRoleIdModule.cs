using Carter;
using MediatR;
using Ticketing.EndPoints.Ticket.Query.GetYearTickerInfo;

namespace Ticketing.EndPoints.Developer.Query.GetDeveloperListByRoleId
{
    public class GetDeveloperListByRoleIdModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
               "api/v1/GetDeveloperListByRoleId",
               async (IMediator mediator, [AsParameters] GetDeveloperListByRoleIdQuery query,
                   CancellationToken cancellationToken) =>
               {
                   return await mediator.Send(query, cancellationToken);
               })
           .WithOpenApi()
           .WithTags("Developer")
           .Produces<object[]>();
        }
    }
}
