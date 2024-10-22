using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;

namespace Ticketing.EndPoints.Group.Query.GetGroup;

public class GetProjectListHandler
{
     public class Handler
    (IProjectService projectService,IProjectRoleService projectRoleService,
        ILogger<GetProjectListHandler> _logger) : IRequestHandler<
        GetProjectListQuery, object>
    {

        public async Task<object> Handle(GetProjectListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectList =new List<Project>();
                var roleList = await projectRoleService.ListAsync(a=>a.RoleId==request.RoleId);
                if (roleList.Count()==0)
                {
                    return await projectService.ListAsync(null);
                }
                foreach (var item in roleList)
                {
                    var projectItem = await projectService.ListAsync(a => a.Id == item.ProjectId);
                    projectList.Add(projectItem.FirstOrDefault());
                }
                return projectList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetGroupList- Handle " + ex.Message);
            }

            return null;
        }
    }
}