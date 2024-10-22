using FastEndpoints;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;

namespace Ticketing_ReprPattern.Endpoints.Ticket.Events;

public class CreateTicketFlowEvent
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public int TicketId { get; set; }
    public int StatusId { get; set; }
    public int CurrentRoleId { get; set; }
    public int PreviousRoleId { get; set; }
    public DateTime InsertDate { get; set; }
}

public class AddTicketFlowEvent : IEventHandler<CreateTicketFlowEvent>
{
    private readonly ITicketService _ticketService;
    private readonly ITicketFlowService _ticketFlowService;
    private readonly ILogger _logger;

    public AddTicketFlowEvent(
        ITicketService ticketService,
        ITicketFlowService ticketFlowService,
        ILogger<AddTicketFlowEvent> logger)
    {
        _ticketService = ticketService;
        _ticketFlowService = ticketFlowService;
        _logger = logger;
    }

    public async Task HandleAsync(
        CreateTicketFlowEvent eventModel, CancellationToken ct)
    {
        try
        {
            await _ticketFlowService.AddAsync(new TicketFlow()
            {
                CurrentRoleId = eventModel.CurrentRoleId,
                InsertDate = DateTime.Now,
                StatusId = eventModel.StatusId,
                UserId = eventModel.UserId,
                TicketId = eventModel.TicketId,
                PreviousRoleId = eventModel.CurrentRoleId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AddTicketFlowEvent -- "+e.Message);
        }
    }
}