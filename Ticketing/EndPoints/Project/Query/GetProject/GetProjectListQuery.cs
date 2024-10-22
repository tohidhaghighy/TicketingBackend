using MediatR;

namespace Ticketing.EndPoints.Group.Query.GetGroup;

public class GetProjectListQuery: IRequest<object>
{
    public int? RoleId { get; set; }
}