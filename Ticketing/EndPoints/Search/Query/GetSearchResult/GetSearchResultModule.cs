using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ticketing.EndPoints.Search.Query.GetSearchResult
{
    public class GetSearchResultModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "api/v1/search",
                    async (IMediator mediator, [FromBody] GetSearchResultQuery query,
                    CancellationToken cancellationToken) =>
                    {
                        return await mediator.Send(query, cancellationToken);
                    })
                .WithOpenApi()
                .WithTags("Search")
                .Produces<object[]>();
        }
    }
}
