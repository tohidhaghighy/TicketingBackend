using Carter;
using MediatR;

namespace Ticketing.EndPoints.Reporting.Query.GetReport
{
    public class GetReportModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "api/v1/downloadReport",
                    async (IMediator mediator, [AsParameters] GetReportQuery query,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);
                        return Results.File(result.FileContents, result.ContentType, result.FileDownloadName);
                    })
                .WithOpenApi()
                .WithTags("Report")
                .Produces<object[]>();
        }
    }
}
