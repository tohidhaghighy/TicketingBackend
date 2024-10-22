using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.Project
{
    public class ProjectRoleService : IProjectRoleService
    {
        public TicketingDbContext DbContext { get; }
        public ProjectRoleService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<ProjectRole> AddAsync(ProjectRole entity)
        {
            var result =  await DbContext.ProjectRoles.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return result.Entity;
        }

        public Task<ProjectRole> UpdateAsync(ProjectRole entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProjectRole entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectRole> GetAsync(Expression<Func<ProjectRole, bool>> expression)
        {
            return await DbContext.ProjectRoles.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<List<ProjectRole>> ListAsync(Expression<Func<ProjectRole, bool>>? expression)
        {
            if (expression == null) return await DbContext.ProjectRoles.ToListAsync();
            return await DbContext.ProjectRoles.Where(expression).ToListAsync();
        }
    }
}
