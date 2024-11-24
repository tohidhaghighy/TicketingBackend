using MediatR;

namespace Ticketing.EndPoints.Ticket.Query.GetYearTickerInfo;

public class GetYearTickerInfoQuery:IRequest<object>
{
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public int RequestTypeId { set; get; }
}