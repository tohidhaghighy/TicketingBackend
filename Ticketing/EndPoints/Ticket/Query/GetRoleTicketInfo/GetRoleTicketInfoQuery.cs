using MediatR;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketInfo;

public class GetRoleTicketInfoQuery:IRequest<object>
{
    public int RoleId { get; set; }
    public int UserId { get; set; }
}