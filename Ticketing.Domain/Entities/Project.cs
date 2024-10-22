using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities;

public class Project : BaseEntity<int>
{
    public string Name { get; set; }
}