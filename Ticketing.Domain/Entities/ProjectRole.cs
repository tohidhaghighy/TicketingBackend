using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities
{
    public class ProjectRole : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public int RoleId { get; set; }
    }
}
