using MediatR;

namespace Ticketing.EndPoints.Massage.Command.AddMassage;

public class AddMassageQuery : IRequest<object>
{
    public int TicketId { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public IFormFile? File { get; set; }
}