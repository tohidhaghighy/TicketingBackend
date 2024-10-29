using Ticketing.Domain.Common;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities;

public class Ticket : BaseEntity<int>
{
    public string TicketNumber { get; set; }
    public int? TicketRowNumber { get; set; }
    public int? ExcelRow { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public Priority Priority { get; set; }
    public int CurrentRoleId { get; set; }
    public string FilePath { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime CloseDate { get; set; }
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public string Username { get; set; }
    public int InsertedRoleId { get; set; }
    public int ProjectId { get; set; }
    public RequestType? RequestTypeId { get; set; }
    public DateTime? LastChangeDatetime { get; set; }
    public string? TicketTime { get; set; }
    public Developer DeveloperId { get; set; }
}