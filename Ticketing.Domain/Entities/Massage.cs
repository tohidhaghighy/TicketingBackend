using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities;

public class Massage : BaseEntity<int>
{
   public Massage()
   {
      
   }
   public int TicketId { get; set; }
   public string Text { get; set; }
   public DateTime InsertDate { get; set; }
   public int UserId { get; set; }
   public string Username { get; set; }
}