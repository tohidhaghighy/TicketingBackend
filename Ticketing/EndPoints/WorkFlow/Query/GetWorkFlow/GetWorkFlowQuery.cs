using MediatR;

namespace Ticketing.EndPoints.WorkFlow.Query.GetWorkFlow
{
    public class GetWorkFlowQuery : IRequest<object>
    {
        public int? TicketId { get; set; }
    }
}
