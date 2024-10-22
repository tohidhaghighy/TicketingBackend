using MediatR;

namespace Ticketing.EndPoints.Massage.Query.GetMassage;

public class GetTicketMassageListQuery:IRequest<object>
{
    public int? TicketId { get; set; }
}