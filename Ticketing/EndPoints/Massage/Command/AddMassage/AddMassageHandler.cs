using MediatR;
using Ticketing.Application.Service.Massage;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Massage.Command.AddMassage;

public class AddMassageHandler
{
    public class Handler(IMassageService massageService,ILogger<AddMassageHandler> _logger):IRequestHandler<AddMassageQuery,object>
    {
        public async Task<object> Handle(AddMassageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string fileName = "";
                if (request.File != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileInfo fileInfo = new FileInfo(request.File.FileName);
                    if (!fileInfo.Extension.Contains("exe"))
                    {
                        fileName = Guid.NewGuid() + fileInfo.Extension;
                        string fileNameWithPath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            request.File.CopyTo(stream);
                        }
                    }
                }
                return await massageService.AddAsync(new Domain.Entities.Massage()
                {
                    Text = request.Text,
                    TicketId = request.TicketId,
                    InsertDate = DateTime.Now,
                    UserId = request.UserId,
                    Username=request.Username,
                    FilePath=fileName,
                });   
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
            }

            return null;
        }
    }
}