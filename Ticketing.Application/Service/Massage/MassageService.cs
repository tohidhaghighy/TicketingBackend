using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Domain.Contracts;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.Massage
{
    public class MassageService : IMassageService
    {
        public MassageService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TicketingDbContext DbContext { get; }


        public async Task<Domain.Entities.Massage> AddAsync(Domain.Entities.Massage entity)
        {
            await DbContext.Massages.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Domain.Entities.Massage entity)
        {
            DbContext.Massages.Remove(entity);
            await DbContext.SaveChangesAsync(true);
            return true;
        }

        public async Task<Domain.Entities.Massage> GetAsync(Expression<Func<Domain.Entities.Massage, bool>> expression)
        {
           return await DbContext.Massages.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<List<Domain.Entities.Massage>> ListAsync(Expression<Func<Domain.Entities.Massage, bool>>? expression)
        {
            if (expression == null) await DbContext.Massages.ToListAsync();
            return await DbContext.Massages.Where(expression).ToListAsync();
        }

        public Task<Domain.Entities.Massage> UpdateAsync(Domain.Entities.Massage entity)
        {
            throw new NotImplementedException();
        }
    }
}
