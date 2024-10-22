using Carter;
using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Query.DownloadFile;

public class DownloadFileresponce
{
    public string filePath { get; set; }
}
public class DownloadFileQuery:IRequest<DownloadFileresponce>
{
    public int ticketId { get; set; }
}
public class DownloadFile
{
    public class Handler(ITicketService ticketService,ILogger<DownloadFile> _logger):IRequestHandler<DownloadFileQuery,DownloadFileresponce>
    {
        public async Task<DownloadFileresponce> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await ticketService.GetAsync(a=>a.Id == request.ticketId);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                string fileNameWithPath = Path.Combine(path, result.FilePath);

                return new DownloadFileresponce()
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
                "api/v1/downloadFile",
                async (IMediator mediator, [AsParameters] DownloadFileQuery query,
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
            .WithTags("Ticket")
            .Produces<object[]>();
    }
}