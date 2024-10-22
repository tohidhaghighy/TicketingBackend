using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Domain.Contracts;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.TicketFlow
{
    public class TicketFlowService : ITicketFlowService
    {
        public TicketFlowService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TicketingDbContext DbContext { get; }

        public async Task<Domain.Entities.TicketFlow> AddAsync(Domain.Entities.TicketFlow entity)
        {
            await DbContext.TicketFlows.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public Task<bool> DeleteAsync(Domain.Entities.TicketFlow entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Domain.Entities.TicketFlow> GetAsync(Expression<Func<Domain.Entities.TicketFlow, bool>> expression)
        {
            return await DbContext.TicketFlows.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<List<Domain.Entities.TicketFlow>> ListAsync(Expression<Func<Domain.Entities.TicketFlow, bool>>? expression)
        {
            return await DbContext.TicketFlows.Where(expression).ToListAsync();
        }

        public Task<Domain.Entities.TicketFlow> UpdateAsync(Domain.Entities.TicketFlow entity)
        {
            throw new NotImplementedException();
        }
    }
}
