using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.ChangeDevelopedBy;
public class DevelopedByHandler
{
    public class Handler(
        ITicketService ticketService,
        ITicketFlowService ticketFlowService,
        ILogger<DevelopedByQuery> _logger) : IRequestHandler<DevelopedByQuery, object>
    {
        public async Task<object> Handle(DevelopedByQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var ticketinfo = await ticketService.GetAsync(a => a.Id == request.TicketId);
                ticketinfo.TicketTime = request.Time;
                ticketinfo.AssignUserId = request.AssignUserId;
                ticketinfo.AssignUserName = request.AssignUserName;

                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = FindCurrentRoleIdByPreviousRoleId(ticketinfo.CurrentRoleId),
                    InsertDate = DateTime.Now,
                    StatusId = ticketinfo.StatusId,
                    UserId = request.AssignUserId,//شناسه کاربر انجام دهنده
                    Username = "ارجاع به" + " " + request.AssignUserName,//نام انجام دهنده
                    TicketId = ticketinfo.Id,
                    PreviousRoleId = ticketinfo.CurrentRoleId
                });

                var result = await ticketService.UpdateAsync(ticketinfo);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : AddTicketHandler- Handle " + ex.Message);
            }

            return null;
        }

        public int FindCurrentRoleIdByPreviousRoleId(int previousRoleId)
        {
            int result = 0;
            switch(previousRoleId)
            {
                case 3: //معاونت آمار
                    result = 1007; //کاربر آمار
                    break;
                case 4: //معاونت زیرساخت، شبکه و امنیت
                    result = 1008; //کاربر زیرساخت، شبکه و امنیت
                    break;
                case 5: //معاونت فناوری اطلاعات
                    result = 1009; //کاربر فناوری اطلاعات
                    break;
            }
            return result;
        }
    }
}


