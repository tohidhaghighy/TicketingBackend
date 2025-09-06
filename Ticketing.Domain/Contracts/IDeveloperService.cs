
using Ticketing.Domain.Common;

namespace Ticketing.Domain.Contracts;

public interface IDeveloperService : IAsyncRepository<Ticketing.Domain.Entities.Developer>
{
}
