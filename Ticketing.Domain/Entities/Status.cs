using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities;

public class Status : BaseEntity<int>
{
    public string Name { get; set; }
}