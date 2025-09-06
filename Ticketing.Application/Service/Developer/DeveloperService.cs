using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ticketing.Domain.Contracts;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.Developer
{
    public class DeveloperService : IDeveloperService
    {
        public DeveloperService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public TicketingDbContext DbContext { get; }
        public Task<Domain.Entities.Developer> AddAsync(Domain.Entities.Developer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Domain.Entities.Developer entity)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Developer> GetAsync(Expression<Func<Domain.Entities.Developer, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Domain.Entities.Developer>> ListAsync(Expression<Func<Domain.Entities.Developer, bool>>? expression)
        {
            if(expression == null) return await DbContext.Developer.ToListAsync();
            return await DbContext.Developer.Where(expression).ToListAsync();
        }

        public Task<Domain.Entities.Developer> UpdateAsync(Domain.Entities.Developer entity)
        {
            throw new NotImplementedException();
        }
    }
}
