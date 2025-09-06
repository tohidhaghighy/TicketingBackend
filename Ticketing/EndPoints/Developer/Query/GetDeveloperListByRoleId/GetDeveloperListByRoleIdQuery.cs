using MediatR;

namespace Ticketing.EndPoints.Developer.Query.GetDeveloperListByRoleId
{
    public class GetDeveloperListByRoleIdQuery : IRequest<object>
    {
        public int RoleId { get; set; }
    }
}
