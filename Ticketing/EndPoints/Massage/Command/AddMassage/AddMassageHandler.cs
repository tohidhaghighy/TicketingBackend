using MediatR;
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
                return await massageService.AddAsync(new Domain.Entities.Massage()
                {
                    Text = request.Text,
                    TicketId = request.TicketId,
                    InsertDate = DateTime.Now,
                    UserId = request.UserId,
                    Username = request.Username
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