using MediatR;
using System.Globalization;
using System.Text.Json;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Ticket.Query.GetYearTickerInfo;

namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public class GetDashboardChartDataHandler
    {
        public class Handler(ITicketService ticketService, ILogger<GetDashboardChartDataHandler> _logger) : IRequestHandler<GetYearTickerInfoQuery, object>
        {
            public async Task<object> Handel(GetYearTickerInfoQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    #region RequestType == Develop
                    var developInRFPTotal = await ticketService.ListAsync(a =>
                                                                         a.StatusId == (int)StatusId.inLine ||
                                                                         a.StatusId == (int)StatusId.inProgress ||
                                                                         a.StatusId == (int)StatusId.awaitingConfirmation &&
                                                                         a.IsSchedule == IsSchedule.yes);

                    var developInRFPDone = await ticketService.ListAsync(a =>
                                                                         a.StatusId == (int)StatusId.awaitingConfirmation &&
                                                                         a.IsSchedule == IsSchedule.yes);

                    var developOutRFPTotal = await ticketService.ListAsync(a =>
                                                                         a.StatusId == (int)StatusId.inLine ||
                                                                         a.StatusId == (int)StatusId.inProgress ||
                                                                         a.StatusId == (int)StatusId.awaitingConfirmation &&
                                                                         a.IsSchedule == IsSchedule.no);

                    var developOutRFPDone = await ticketService.ListAsync(a =>
                                                                         a.StatusId == (int)StatusId.awaitingConfirmation &&
                                                                         a.IsSchedule == IsSchedule.no);
                    #endregion

                    #region RequestType == Support
                    int CompanyCommitmentToSupportTime = 850;
                    var SupportTicket = await ticketService.ListAsync(a =>
                                                                      a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                      a.StatusId == (int)StatusId.awaitingRejecting ||
                                                                      a.StatusId == (int)StatusId.done ||
                                                                      a.StatusId == (int)StatusId.rejected);
                    #endregion

                    #region Developer Data

                    #region All Tickets
                    var AllTickets = await ticketService.ListAsync(null);
                    #endregion

                    #region p_rezayeh
                    var p_rezayehTicket = AllTickets.Where(a => a.DeveloperId == Developer.p_rezayeh);
                    #endregion

                    #region m_bagher
                    var m_bagherTicket = AllTickets.Where(a => a.DeveloperId == Developer.m_bagheri);
                    #endregion

                    #region t_hagigi
                    var t_hagigiTicket = AllTickets.Where(a => a.DeveloperId == Developer.t_hagigi);
                    #endregion

                    #region m_borji
                    var m_borjiTicket = AllTickets.Where(a => a.DeveloperId == Developer.m_borji);
                    #endregion

                    #region m_salehi
                    var m_salehiTicket = AllTickets.Where(a => a.DeveloperId == Developer.m_salehi);
                    #endregion

                    #region Sh_kazempour
                    var Sh_kazempourTicket = AllTickets.Where(a => a.DeveloperId == Developer.Sh_kazempour);
                    #endregion

                    #region e_darvishi
                    var e_darvishiTicket = AllTickets.Where(a => a.DeveloperId == Developer.e_darvishi);
                    #endregion

                    #region e_ebrahimi
                    var e_ebrahimiTicket = AllTickets.Where(a => a.DeveloperId == Developer.e_ebrahimi);
                    #endregion

                    #region s_mohamadzadeh
                    var s_mohamadzadehTicket = AllTickets.Where(a => a.DeveloperId == Developer.s_mohamadzadeh);
                    #endregion

                    #endregion

                    //Return//

                    #region Return Variables

                    var developResultYearInRFP = new List<MonthResult>();
                    var developResultYearOutRFP = new List<MonthResult>();
                    var supportResultYear = new List<MonthResult>();
                    var developerResultYear = new List<MonthResult>();
                    var p = new PersianCalendar();

                    #endregion

                    #region Developed result
                    for (var i = 1; i < 13; i++)
                    {
                        developResultYearInRFP.Add(new MonthResult
                        {
                            Month = GetMonth(i),
                            Total = developInRFPTotal.Count(a => p.GetMonth(a.InsertDate) == i),
                            Done = developInRFPDone.Count(a => p.GetMonth(a.InsertDate) == i)
                        });

                        developResultYearOutRFP.Add(new MonthResult
                        {
                            Month = GetMonth(i),
                            Total = developOutRFPTotal.Count(a => p.GetMonth(a.InsertDate) == i),
                            Done = developOutRFPDone.Count(a => p.GetMonth(a.InsertDate) == i)
                        });
                    }
                    #endregion

                    #region support year result
                    for (var i = 1; i < 13; i++)
                    {
                        var tickets = SupportTicket.Where(a => p.GetMonth(a.InsertDate) == i);
                        supportResultYear.Add(new MonthResult
                        {
                            Month = GetMonth(i),
                            CompanyCommitmentTime = CompanyCommitmentToSupportTime,
                            SupportTime = tickets.Sum(item => int.TryParse(item.TicketTime, out var time) ? time : 0)
                        });
                    }
                    #endregion

                    #region Developer Data year result
                    for (var i = 1; i < 13; i++)
                    {
                        var monthResult = new MonthResult { Month = GetMonth(i) };

                        var developers = new Dictionary<string, IEnumerable<Domain.Entities.Ticket>>
                        {
                            { "p_rezayeh", p_rezayehTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "m_bagheri", m_bagherTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "t_hagigi", t_hagigiTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "m_borji", m_borjiTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "m_salehi", m_salehiTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "Sh_kazempour", Sh_kazempourTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "e_darvishi", e_darvishiTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "e_ebrahimi", e_ebrahimiTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                            { "s_mohamadzadeh", s_mohamadzadehTicket.Where(a => p.GetMonth(a.InsertDate) == i) },
                        };

                        foreach (var developer in developers)
                        {
                            monthResult.DeveloperCounts[developer.Key] = developer.Value.Count();
                            monthResult.DeveloperTimes[developer.Key] = developer.Value.Sum(item => int.TryParse(item.TicketTime, out var ticketTime) ? ticketTime : 0);
                        }
                    }
                    #endregion

                    var result = new
                    {
                        DevelopResultYearInRFP = developResultYearInRFP,
                        DevelopResultYearOutRFP = developResultYearOutRFP,
                        SupportResultYear = supportResultYear,
                        DeveloperResultYear = developerResultYear
                    };

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

            public Task<object> Handle(GetYearTickerInfoQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
        public class MonthResult
        {
            public string Month { get; set; }
            public int Total { get; set; } // For developInRFPTotal and developOutRFPTotal
            public int Done { get; set; }  // For developInRFPDone and developOutRFPDone
            public int SupportTime { get; set; } // For support year result
            public int CompanyCommitmentTime { get; set; } // For support year result
            public Dictionary<string, int> DeveloperCounts { get; set; } = new(); // Developer ticket counts
            public Dictionary<string, int> DeveloperTimes { get; set; } = new();  // Developer ticket times
        }
    }
}
