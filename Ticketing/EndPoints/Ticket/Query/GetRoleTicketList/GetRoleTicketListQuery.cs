using MediatR;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;

public class GetRoleTicketListQuery:IRequest<object>
{
    public int RoleId { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
}