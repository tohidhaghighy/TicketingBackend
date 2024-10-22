using Ticketing.Domain.Common;
using Ticketing.Domain.Entities;

namespace Ticketing.Domain.Contracts;

public interface IStatusService: IAsyncRepository<Entities.Status>
{

}