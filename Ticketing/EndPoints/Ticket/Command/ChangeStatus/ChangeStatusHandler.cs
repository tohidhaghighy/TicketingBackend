using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.ChangeStatus;

public class ChangeStatusHandler
{
    public class Handler(ITicketService ticketService,ITicketFlowService ticketFlowService,ILogger<ChangeStatusHandler> _logger):IRequestHandler<ChangeStatusQuery,object>
    {
        public async Task<object> Handle(ChangeStatusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var findticket = await ticketService.GetAsync(a => a.Id == request.TicketId);
                if (request.Status == (int)StatusId.awaitingConfirmation || request.Status == (int)StatusId.awaitingRejecting)
                {
                    if (findticket.ProcessEndDateTime == null)
                    {
                        findticket.ProcessEndDateTime = DateTime.Now;
                        findticket.StatusId = request.Status;
                    }
                    else
                    {
                        findticket.StatusId = request.Status;
                    }
                }
                if (request.Status == (int)StatusId.awaitingRejecting)
                {
                    if (findticket.ProcessEndDateTime == null)
                    {
                        findticket.ProcessEndDateTime = DateTime.Now;
                        findticket.StatusId = request.Status;
                    }
                    else
                    {
                        findticket.StatusId = request.Status;
                    }
                }
                else if(request.Status == (int)StatusId.done)
                {
                    findticket.CloseDate = DateTime.Now;
                    findticket.StatusId = request.Status;
                }
                else
                {
                    findticket.StatusId = request.Status;
                }
                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = findticket.InsertedRoleId,
                    InsertDate = DateTime.Now,
                    StatusId = findticket.StatusId,
                    Username = FindUserName(findticket , request),
                    UserId = request.UserId,
                    TicketId = findticket.Id,
                    PreviousRoleId = findticket.CurrentRoleId
                });
                return await ticketService.UpdateAsync(findticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : ChangeStatusHandler- Handle " + ex.Message);
            }

            return null;
        }

        public string FindUserName(Domain.Entities.Ticket ticket , ChangeStatusQuery request)
        {
            string userName = null;

            #region معاونت آمار
            if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminsta &&
                request.UserId == 1059 && //AdminSta userId
                (ticket.StatusId == (int)Domain.Enums.StatusId.inLine || 
                ticket.StatusId == (int)Domain.Enums.StatusId.inProgress)
              )
            {
                userName = "در حال انجام توسط معاونت آمار";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminsta &&
                request.UserId == 1059 && //AdminSta userId
                ticket.StatusId == (int)Domain.Enums.StatusId.done
              )
            {
                userName = "اتمام تیکت توسط معاونت آمار";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminsta &&
                request.UserId == 1059 && //AdminSta userId
                ticket.StatusId == (int)Domain.Enums.StatusId.rejected
              )
            {
                userName = "رد شده توسط معاونت آمار";
            }
            #endregion

            #region معاون فناوری اطلاعات
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminita &&
                request.UserId == 1060 && //AdminIta userId
                (ticket.StatusId == (int)Domain.Enums.StatusId.inLine ||
                ticket.StatusId == (int)Domain.Enums.StatusId.inProgress)
              )
            {
                userName = "در حال انجام توسط معاون فناوری اطلاعات";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminita &&
                request.UserId == 1060 && //AdminIta userId
               ticket.StatusId == (int)Domain.Enums.StatusId.done
             )
            {
                userName = "اتمام تیکت توسط معاون فناوری اطلاعات";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminita &&
                request.UserId == 1060 && //AdminIta userId
                ticket.StatusId == (int)Domain.Enums.StatusId.rejected
              )
            {
                userName = "رد شده توسط معاون فناوری اطلاعات";
            }
            #endregion

            #region معاون زیرساخت، شبکه و امنیت
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminina &&
                request.UserId == 1061 && //AdminIna userId
                (ticket.StatusId == (int)Domain.Enums.StatusId.inLine ||
                ticket.StatusId == (int)Domain.Enums.StatusId.inProgress)
              )
            {
                userName = "در حال انجام توسط معاونت زیرساخت، شبکه و امنیت";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminina &&
                request.UserId == 1061 && //AdminIna userId
               ticket.StatusId == (int)Domain.Enums.StatusId.done
             )
            {
                userName = "اتمام تیکت توسط معاون زیرساخت، شبکه و امنیت";
            }
            else if (ticket.CurrentRoleId == (int)Domain.Enums.Role.adminina &&
                request.UserId == 1061 && //AdminIna userId
                ticket.StatusId == (int)Domain.Enums.StatusId.rejected
              )
            {
                userName = "رد شده توسط معاون زیرساخت، شبکه و امنیت";
            }
            #endregion

            #region کارمندان
            else
            {
                if(ticket.StatusId == (int)Domain.Enums.StatusId.done)
                {
                    userName = "اتمام تیکت توسط" + " " + ticket.AssignUserName;
                }
                else if (ticket.StatusId == (int)Domain.Enums.StatusId.rejected)
                {
                    userName = "رد شده توسط" + " " + ticket.AssignUserName;
                }
                else
                {
                    userName = "در حال انجام توسط" + " " + ticket.AssignUserName;
                }
            }
            #endregion

            return userName;
        }
    }
}