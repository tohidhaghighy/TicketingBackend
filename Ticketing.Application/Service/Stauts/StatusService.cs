using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Database;

namespace Ticketing.Application.Service.Stauts
{
    public class StatusService : IStatusService
    {
        public StatusService(TicketingDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TicketingDbContext DbContext { get; }

        public Task<Status> AddAsync(Status entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Status entity)
        {
            throw new NotImplementedException();
        }

        public Task<Status> GetAsync(Expression<Func<Status, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Status>> ListAsync(Expression<Func<Status, bool>>? expression)
        {
            return await DbContext.Statuses.ToListAsync();
        }

        public Task<Status> UpdateAsync(Status entity)
        {
            throw new NotImplementedException();
        }
    }
}
