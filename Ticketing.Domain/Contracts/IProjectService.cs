using Ticketing.Domain.Common;

namespace Ticketing.Domain.Contracts;

public interface IProjectService: IAsyncRepository<Entities.Project>
{
    
}