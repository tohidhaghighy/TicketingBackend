using MediatR;
using System.Globalization;
using System.Text.Json;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public partial class GetDashboardChartDataHandler
    {
        public class Handler(ITicketService ticketService, ILogger<GetDashboardChartDataHandler> _logger) : IRequestHandler<GetDashboardChartDataQuery, object>
        {
            public async Task<object> Handle(GetDashboardChartDataQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    #region RequestType == Develop Tickets
                    var developInRFPTotal = await ticketService.ListAsync(a => a.IsSchedule == IsSchedule.yes);
                    var developInRFPTotalTickets = new List<Domain.Entities.Ticket>();
                    developInRFPTotalTickets = developInRFPTotal.Where(a => 
                                                                           a.StatusId == (int)StatusId.inLine ||
                                                                           a.StatusId == (int)StatusId.inProgress ||
                                                                           a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                           a.StatusId == (int)StatusId.done).ToList();

                    var developInRFPDone = await ticketService.ListAsync(a => a.IsSchedule == IsSchedule.yes);
                    var developInRFPDoneTicket = new List<Domain.Entities.Ticket>();
                    developInRFPDoneTicket = developInRFPDone.Where(a =>
                                                                         a.IsSchedule == IsSchedule.yes &&
                                                                         a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                         a.StatusId == (int)StatusId.done).ToList();

                    var developOutRFPTotal = await ticketService.ListAsync(a => a.IsSchedule == IsSchedule.no);
                    var developOutRFPTotalTickets = new List<Domain.Entities.Ticket>();
                    developOutRFPTotalTickets = developOutRFPTotal.Where(a =>
                                                                        a.StatusId == (int)StatusId.inLine ||
                                                                        a.StatusId == (int)StatusId.inProgress ||
                                                                        a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                        a.StatusId == (int)StatusId.done).ToList();

                    var developOutRFPDone = await ticketService.ListAsync(a => a.IsSchedule == IsSchedule.no);
                    var developOutRFPDoneTickets = new List<Domain.Entities.Ticket>();
                    developOutRFPDoneTickets = developOutRFPDone.Where(a =>
                                                                      a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                      a.StatusId == (int)StatusId.done).ToList();
                    #endregion

                    #region RequestType == Support Tickets
                    // TODO: Change it to dynamic value 
                    int CompanyCommitmentToSupportTime = 1160;
                    var Support= await ticketService.ListAsync(a => a.IsSchedule == IsSchedule.Support);
                    var SupportTicket = new List<Domain.Entities.Ticket>();
                    SupportTicket = Support.Where(a =>
                                                  a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                  a.StatusId == (int)StatusId.awaitingRejecting ||
                                                  a.StatusId == (int)StatusId.done ||
                                                  a.StatusId == (int)StatusId.rejected).ToList();

                    #endregion

                    #region Developer Tickets
                    var AllTickets = await ticketService.ListAsync(null);

                    var developers = Enum.GetValues(typeof(Developer)).Cast<Developer>()
                                     .Where(dev => dev != Developer.all && dev != Developer.unknown)
                                     .ToDictionary(dev => dev.ToString(), dev => AllTickets.Where(a => a.DeveloperId == dev));
                    #endregion

                    //~~~~~~~~~~~~~~~~~~~~~~~~Return~~~~~~~~~~~~~~~~~~~~~~~~//

                    #region Return Variables

                    var developResultYearInRFP = new List<DevelopResultYear>();
                    var developResultYearOutRFP = new List<DevelopResultYear>();
                    var supportResultYear = new List<SupportResultYear>();
                    var developerResultYear = new List<DeveloperResultYear>();
                    var p = new PersianCalendar();

                    #endregion

                    #region Developed result
                    for (var i = 1; i < 13; i++)
                    {
                        developResultYearInRFP.Add(new DevelopResultYear
                        {
                            Month = GetMonth(i),
                            Total = developInRFPTotalTickets.Count(a => p.GetMonth(a.InsertDate) == i),
                            Done = developInRFPDoneTicket.Count(a => p.GetMonth(a.InsertDate) == i)
                        });

                        developResultYearOutRFP.Add(new DevelopResultYear
                        {
                            Month = GetMonth(i),
                            Total = developOutRFPTotalTickets.Count(a => p.GetMonth(a.InsertDate) == i),
                            Done = developOutRFPDoneTickets.Count(a => p.GetMonth(a.InsertDate) == i)
                        });
                    }
                    #endregion

                    #region support year result
                    for (var i = 1; i < 13; i++)
                    {
                        var tickets = SupportTicket.Where(a => p.GetMonth(a.InsertDate) == i);
                        var totalTime = 0;
                        foreach(var ticket in tickets)
                        {
                            if(int.TryParse(ticket.TicketTime, out var time))
                            {
                                totalTime += time;
                            }
                        }
                        supportResultYear.Add(new SupportResultYear
                        {
                            Month = GetMonth(i),
                            CompanyCommitmentTime = CompanyCommitmentToSupportTime,
                            SupportTime = totalTime,
                        });
                    }
                    #endregion

                    #region Developer Data year result
                    var developerResultCount = new List<List<DeveloperData>>();
                    var developerResultTime = new List<List<DeveloperData>>();
                    for (var i = 1; i < 13; i++)
                    {
                        var monthDataCount = new List<DeveloperData>();
                        var monthDataTime = new List<DeveloperData>();
                        foreach (var developer in developers)
                        {
                            var tickets = developer.Value.Where(a => p.GetMonth(a.InsertDate) == i);
                            monthDataCount.Add(new DeveloperData()
                            {
                                name= developer.Key,
                                value= tickets.Count()
                            });

                            int totalTime = 0;
                            foreach(var ticket in tickets)
                            {
                                if (int.TryParse(ticket.TicketTime, out var time))
                                {
                                    totalTime += time;
                                }
                            }
                            monthDataTime.Add(new DeveloperData()
                            {
                                name = developer.Key,
                                value = totalTime
                            });
                        }
                        developerResultCount.Add(monthDataCount);
                        developerResultTime.Add(monthDataTime);
                    }

                    #endregion

                    #region result
                    var result = new
                    {
                        DevelopResultYearInRFP = developResultYearInRFP,
                        DevelopResultYearOutRFP = developResultYearOutRFP,
                        SupportResultYear = supportResultYear,
                        DeveloperResultCount = developerResultCount,
                        DeveloperResultTime = developerResultTime
                    };
                    #endregion

                    return JsonSerializer.Serialize(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Get Error : GetTicketMassageList- Handle " + ex.Message);
                }

                return null;
            }

            #region GetMonth method
            public string GetMonth(int month)
            {
                if (month == 1) return "فروردین";
                else if (month == 2) return "اردیبهشت";
                else if (month == 3) return "خرداد";
                else if (month == 4) return "تیر";
                else if (month == 5) return "مرداد";
                else if (month == 6) return "شهریور";
                else if (month == 7) return "مهر";
                else if (month == 8) return "ابان";
                else if (month == 9) return "اذر";
                else if (month == 10) return "دی";
                else if (month == 11) return "بهمن";
                else if (month == 12) return "اسفند";
                return "";
            }

            #endregion
        }
    }
}
