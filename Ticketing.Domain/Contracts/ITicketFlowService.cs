using Ticketing.Domain.Common;

namespace Ticketing.Domain.Contracts;

public interface ITicketFlowService: IAsyncRepository<Entities.TicketFlow>
{
    
}