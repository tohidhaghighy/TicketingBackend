using Carter;
using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.EndPoints.Ticket.Query.DownloadFile;

namespace Ticketing.EndPoints.Massage.Query.DownloadFile;

public class MasaageDownloadFileResponce
{
    public string filePath { get; set; }
}
public class MassageDownloadFileQuery : IRequest<MasaageDownloadFileResponce>
{
    public int massageId { get; set; }
}
public class DownloadMessageFile
{
    public class Handler(IMassageService massageService, ILogger<DownloadMessageFile> _logger) : IRequestHandler<MassageDownloadFileQuery, MasaageDownloadFileResponce>
    {
        public async Task<MasaageDownloadFileResponce> Handle(MassageDownloadFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await massageService.GetAsync(a => a.Id == request.massageId);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                string fileNameWithPath = Path.Combine(path, result.FilePath);

                return new MasaageDownloadFileResponce()
                {
                    filePath = fileNameWithPath,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError("Get Error : DownloadFile- Handle " + ex.Message);
            }

            return null;
        }
    }
}
public class DownloadFileModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/v1/downloadMassageFile",
                async (IMediator mediator, [AsParameters] MassageDownloadFileQuery query,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(query, cancellationToken);
                    if (System.IO.File.Exists(result.filePath))
                    {
                        return Results.File(System.IO.File.OpenRead(result.filePath), "application/octet-stream", Path.GetFileName(result.filePath));
                    }
                    return null;
                })
            .WithOpenApi()
            .WithTags("Massage")
            .Produces<object[]>();
    }
}
