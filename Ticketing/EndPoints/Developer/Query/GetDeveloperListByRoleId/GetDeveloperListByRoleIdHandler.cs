using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Developer.Query.GetDeveloperListByRoleId
{
    public class GetDeveloperListByRoleIdHandler
    {
        public class Handler
            (IDeveloperService developerService , 
            ILogger<GetDeveloperListByRoleIdHandler> _logger) : IRequestHandler
            <GetDeveloperListByRoleIdQuery , object>
        {
            public async Task<object> Handle(GetDeveloperListByRoleIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var developerList = await developerService.ListAsync(a => a.RoleId == request.RoleId);
                    return developerList;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Get Error : GetDeveloperListByRoleId- Handle " + ex.Message);
                }

                return null;
            }
        }
    }
}
