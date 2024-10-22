using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ticketing.Domain.Contracts;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.Project
{
    public class ProjectService : IProjectService
    {
        public ProjectService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TicketingDbContext DbContext { get; }
        public Task<Domain.Entities.Project> AddAsync(Domain.Entities.Project entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Domain.Entities.Project entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Domain.Entities.Project> GetAsync(Expression<Func<Domain.Entities.Project, bool>> expression)
        {
            return await DbContext.Projects.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<List<Domain.Entities.Project>> ListAsync(Expression<Func<Domain.Entities.Project, bool>>? expression)
        {
            if (expression == null) return await DbContext.Projects.ToListAsync();
            return await DbContext.Projects.Where(expression).ToListAsync();
        }

        public Task<Domain.Entities.Project> UpdateAsync(Domain.Entities.Project entity)
        {
            throw new NotImplementedException();
        }
    }
}
