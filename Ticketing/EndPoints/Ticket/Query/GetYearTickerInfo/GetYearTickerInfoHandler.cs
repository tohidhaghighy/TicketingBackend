using MediatR;
using System.Globalization;
using Ticketing.Application.Service.Stauts;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.EndPoints.Ticket.DTOs;
using Ticketing.EndPoints.Ticket.Query.GetYearTickerInfo;

namespace Ticketing.EndPoints.Ticket.Query.GetGroupTicketList;
public class GetYearTickerInfoHandler
{
    public class Handler(ITicketService ticketService,IProjectService projectService ,IStatusService statusService,ILogger<GetYearTickerInfoHandler> _logger):IRequestHandler<GetYearTickerInfoQuery, object>
    {
        public async Task<object> Handle(GetYearTickerInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var doneList = new List<int>();
                var rejectList = new List<int>();
                var doingList = new List<int>();
                var resultyear = new List<MonthTicketItem>();
                var p = new PersianCalendar();
                var result = await ticketService.ListAsync(a => (a.UserId == request.UserId) ||
                                                                                    a.CurrentRoleId == request.RoleId);
                if (request.RoleId == 4)
                {
                     result = await ticketService.ListAsync(null);
                }
                else if (request.RoleId == 5)
                {
                    result = await ticketService.ListAsync(a => a.StatusId != 2 || a.UserId == request.UserId);
                }

                for (var i = 1; i < 13; i++)
                {
                    int doneCount = result.Where(a=>p.GetMonth(a.InsertDate) == i && a.StatusId==1).Count();
                    int rejectCount = result.Where(a=> p.GetMonth(a.InsertDate) == i && a.StatusId==4).Count();
                    int doningCount = result.Where(a=> p.GetMonth(a.InsertDate) == i && a.StatusId==3).Count();
                    int newCount = result.Where(a => p.GetMonth(a.InsertDate) == i && a.StatusId == 2).Count();
                    resultyear.Add(new MonthTicketItem() {
                        Month = GetMonth(i),
                        Value = doneCount+ rejectCount+ doningCount+ newCount,
                    });
                }

                return resultyear;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
            }

            return null;
        }

        public string GetMonth(int month)
        {
            if (month == 1) return "فروردین";
            else if(month == 2) return "اردیبهشت";
            else if(month == 3) return "خرداد";
            else if(month == 4) return "تیر";
            else if(month == 5) return "مرداد";
            else if(month == 6) return "شهریور";
            else if(month == 7) return "مهر";
            else if(month == 8) return "ابان";
            else if(month == 9) return "اذر";
            else if(month == 10) return "دی";
            else if(month == 11) return "بهمن";
            else if (month == 12) return "اسفند";
            return "";
        }
    }
}