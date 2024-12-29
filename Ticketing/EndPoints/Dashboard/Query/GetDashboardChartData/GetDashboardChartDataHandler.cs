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
                    #region Develop in RFB
                    var developInRFPTicket = await ticketService.ListAsync(a => a.RequestTypeId == RequestType.Develop && 
                                                                           a.IsSchedule == IsSchedule.yes &&
                                                                          (a.StatusId == (int)StatusId.inLine ||
                                                                           a.StatusId == (int)StatusId.inProgress ||
                                                                           a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                           a.StatusId == (int)StatusId.done));

                    var developInRFPDoneTicket = await ticketService.ListAsync(a => a.RequestTypeId == RequestType.Develop && 
                                                                               a.IsSchedule == IsSchedule.yes &&
                                                                              (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                               a.StatusId == (int)StatusId.done));
                    #endregion

                    #region Develop out RFB
                    var developOutRFPTicket = await ticketService.ListAsync(a => a.RequestTypeId == RequestType.Develop &&
                                                                            a.IsSchedule == IsSchedule.no &
                                                                           (a.StatusId == (int)StatusId.inLine ||
                                                                            a.StatusId == (int)StatusId.inProgress ||
                                                                            a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                            a.StatusId == (int)StatusId.done));

                    var developOutRFPDoneTickets = await ticketService.ListAsync(a => a.RequestTypeId == RequestType.Develop && 
                                                                                 a.IsSchedule == IsSchedule.no &&
                                                                                (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                                 a.StatusId == (int)StatusId.done));
                    #endregion

                    #region Support Tickets
                    // TODO: Change it to dynamic value 
                    int CompanyCommitmentToSupportTime = 1160;
                    var SupportTicket = await ticketService.ListAsync(a => a.RequestTypeId == RequestType.Support);
                    #endregion

                    #region Developer Tickets
                    var AllValidTickets = await ticketService.ListAsync(a => a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                             a.StatusId == (int)StatusId.done);

                    var developers = Enum.GetValues(typeof(Developer)).Cast<Developer>()
                                     .Where(dev => dev != Developer.all && dev != Developer.unknown)
                                     .ToDictionary(dev => dev.ToString(), dev => AllValidTickets.Where(a => a.DeveloperId == dev));

                    var p_rezayehTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.p_rezayeh && 
                                                                             (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                              a.StatusId== (int)StatusId.done));

                    var m_bagheriTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.m_bagheri &&
                                                                             (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                              a.StatusId == (int)StatusId.done));

                    var t_hagigiTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.t_hagigi &&
                                                                            (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                             a.StatusId == (int)StatusId.done));

                    var m_borjiTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.m_borji &&
                                                                           (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                            a.StatusId == (int)StatusId.done));

                    var m_salehiTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.m_salehi &&
                                                                            (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                             a.StatusId == (int)StatusId.done));

                    var Sh_kazempourTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.Sh_kazempour &&
                                                                           (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                            a.StatusId == (int)StatusId.done));

                    var e_darvishiTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.e_darvishi &&
                                                                              (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                               a.StatusId == (int)StatusId.done));

                    var s_mohamadzadehTickets = await ticketService.ListAsync(a => a.DeveloperId == Developer.s_mohamadzadeh &&
                                                                             (a.StatusId == (int)StatusId.awaitingConfirmation ||
                                                                              a.StatusId == (int)StatusId.done));
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
                            Total = developInRFPTicket.Count(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == i),
                            Done = developInRFPDoneTicket.Count(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == i)
                        });

                        developResultYearOutRFP.Add(new DevelopResultYear
                        {
                            Month = GetMonth(i),
                            Total = developOutRFPTicket.Count(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == i),
                            Done = developOutRFPDoneTickets.Count(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == i)
                        });
                    }
                    #endregion

                    #region support year result
                    for (var i = 1; i < 13; i++)
                    {
                        var tickets = SupportTicket.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == i);
                        var totalTime = 0;
                        foreach (var ticket in tickets)
                        {
                            if (int.TryParse(ticket.TicketTime, out var time))
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

                    #region return lists
                    var monthDataCount_support = new List<DeveloperData>();
                    var monthDataTime_support = new List<DeveloperData>();

                    var monthDataCount_develop = new List<DeveloperData>();
                    var monthDataTime_develop = new List<DeveloperData>();
                    #endregion

                    #region p_rezayeh
                    var p_rezayehSupport = p_rezayehTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var p_rezayehDevelop = p_rezayehTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "p_rezayeh",
                        value = p_rezayehSupport.Count()
                    });

                    int p_rezayehSupportTime = p_rezayehSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_support.Add(new DeveloperData()
                    {
                        name = "p_rezayeh",
                        value = p_rezayehSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "p_rezayeh",
                        value = p_rezayehDevelop.Count()
                    });

                    int p_rezayehDevelopTime = p_rezayehDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "p_rezayeh",
                        value = p_rezayehDevelopTime
                    });
                    #endregion

                    #endregion

                    #region m_bagheri
                    var m_bagheriSupport = m_bagheriTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var m_bagheriDevelop = m_bagheriTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "m_bagheri",
                        value = m_bagheriSupport.Count()
                    });

                    int m_bagheriSupportTime = p_rezayehSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_support.Add(new DeveloperData()
                    {
                        name = "m_bagheri",
                        value = m_bagheriSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "m_bagheri",
                        value = m_bagheriDevelop.Count()
                    });

                    int m_bagheriDevelopTime = m_bagheriDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "m_bagheri",
                        value = m_bagheriDevelopTime
                    });
                    #endregion

                    #endregion

                    #region t_hagigi
                    var t_hagigiSupport = t_hagigiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var t_hagigiDevelop = t_hagigiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "t_hagigi",
                        value = t_hagigiSupport.Count()
                    });

                    int t_hagigiSupportTime = t_hagigiSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "t_hagigi",
                        value = t_hagigiSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "t_hagigi",
                        value = m_bagheriDevelop.Count()
                    });

                    int t_hagigiDevelopTime = t_hagigiDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "t_hagigi",
                        value = t_hagigiDevelopTime
                    });
                    #endregion

                    #endregion

                    #region m_borji
                    var m_borjiSupport = m_borjiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var m_borjiDevelop = m_borjiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "m_borji",
                        value = m_borjiSupport.Count()
                    });

                    int m_borjiSupportTime = m_borjiSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "m_borji",
                        value = m_borjiSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "m_borji",
                        value = m_borjiDevelop.Count()
                    });

                    int m_borjiDevelopTime = m_borjiDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "m_borji",
                        value = m_borjiDevelopTime
                    });
                    #endregion

                    #endregion

                    #region m_salehi
                    var m_salehiSupport = m_salehiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var m_salehiDevelop = m_salehiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "m_salehi",
                        value = m_salehiSupport.Count()
                    });

                    int m_salehiSupportTime = m_salehiSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "m_salehi",
                        value = m_salehiSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "m_salehi",
                        value = m_salehiDevelop.Count()
                    });

                    int m_salehiDevelopTime = m_salehiDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "m_salehi",
                        value = m_salehiDevelopTime
                    });
                    #endregion

                    #endregion

                    #region Sh_kazempour
                    var Sh_kazempourSupport = Sh_kazempourTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var Sh_kazempourDevelop = Sh_kazempourTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "Sh_kazempour",
                        value = Sh_kazempourSupport.Count()
                    });

                    int Sh_kazempourSupportTime = Sh_kazempourSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "Sh_kazempour",
                        value = Sh_kazempourSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "Sh_kazempour",
                        value = Sh_kazempourDevelop.Count()
                    });

                    int Sh_kazempourDevelopTime = Sh_kazempourDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "Sh_kazempour",
                        value = Sh_kazempourDevelopTime
                    });
                    #endregion

                    #endregion

                    #region e_darvishi
                    var e_darvishiSupport = e_darvishiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var e_darvishiDevelop = e_darvishiTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "e_darvishi",
                        value = e_darvishiSupport.Count()
                    });

                    int e_darvishiSupportTime = e_darvishiSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "e_darvishi",
                        value = Sh_kazempourSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "e_darvishi",
                        value = e_darvishiDevelop.Count()
                    });

                    int e_darvishiDevelopTime = e_darvishiDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "e_darvishi",
                        value = Sh_kazempourDevelopTime
                    });
                    #endregion

                    #endregion

                    #region s_mohamadzadeh
                    var s_mohamadzadehSupport = s_mohamadzadehTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);
                    var s_mohamadzadehDevelop = s_mohamadzadehTickets.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    #region Support
                    monthDataCount_support.Add(new DeveloperData()
                    {
                        name = "s_mohamadzadeh",
                        value = s_mohamadzadehSupport.Count()
                    });

                    int s_mohamadzadehSupportTime = s_mohamadzadehSupport.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "s_mohamadzadeh",
                        value = s_mohamadzadehSupportTime
                    });
                    #endregion

                    #region Develop
                    monthDataCount_develop.Add(new DeveloperData()
                    {
                        name = "s_mohamadzadeh",
                        value = s_mohamadzadehDevelop.Count()
                    });

                    int s_mohamadzadehDevelopTime = s_mohamadzadehDevelop.Sum(ticket => int.Parse(ticket.TicketTime));
                    monthDataTime_develop.Add(new DeveloperData()
                    {
                        name = "s_mohamadzadeh",
                        value = s_mohamadzadehDevelopTime
                    });
                    #endregion

                    #endregion

                    #region old query

                    //foreach (var developer in developers)
                    //{
                    //    var supportTickets = developer.Value.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Support);

                    //    var developTickets = developer.Value.Where(a => p.GetMonth((DateTime)a.ProcessEndDateTime) == request.monthId && a.RequestTypeId == RequestType.Develop);

                    //    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

                    //    #region Support

                    //    monthDataCount_support.Add(new DeveloperData()
                    //    {
                    //        name = developer.Key,
                    //        value = supportTickets.Count()
                    //    });

                    //    int supportTotalTime = supportTickets
                    //        .Where(ticket => ticket.ProcessEndDateTime.HasValue) // Only include tickets with ProcessEndDateTime
                    //        .Where(ticket => int.TryParse(ticket.TicketTime, out _)) // Ensure TicketTime can be parsed as int
                    //        .Sum(ticket => int.Parse(ticket.TicketTime)); // Sum up valid TicketTime values

                    //    monthDataTime_support.Add(new DeveloperData()
                    //    {
                    //        name = developer.Key,
                    //        value = supportTotalTime
                    //    });

                    //    #endregion

                    //    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

                    //    #region Develop

                    //    monthDataCount_develop.Add(new DeveloperData()
                    //    {
                    //        name = developer.Key,
                    //        value = developTickets.Count()
                    //    });

                    //    int developTotalTime = developTickets
                    //        .Where(ticket => ticket.ProcessEndDateTime.HasValue) // Only include tickets with ProcessEndDateTime
                    //        .Where(ticket => int.TryParse(ticket.TicketTime, out _)) // Ensure TicketTime can be parsed as int
                    //        .Sum(ticket => int.Parse(ticket.TicketTime)); // Sum up valid TicketTime values

                    //    monthDataTime_develop.Add(new DeveloperData()
                    //    {
                    //        name = developer.Key,
                    //        value = developTotalTime
                    //    });

                    //    #endregion
                    //}
                    #endregion

                    #endregion

                    #region result
                    var result = new
                    {
                        DevelopResultYearInRFP = developResultYearInRFP,
                        DevelopResultYearOutRFP = developResultYearOutRFP,
                        SupportResultYear = supportResultYear,
                        DeveloperResultCount_support = monthDataCount_support,
                        DeveloperResultTime_support = monthDataTime_support,
                        DeveloperResultCount_develop = monthDataCount_develop,
                        DeveloperResultTime_develop = monthDataTime_develop
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
