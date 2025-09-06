using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities
{
    public class Developer : BaseEntity<int>
    {
        public Developer()
        {
            
        }
        public string Name { get; set; }
        public int RoleId { get; set; }
    }
}
