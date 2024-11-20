using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Reporting.Query.Dtos;
using Ticketing.EndPoints.Reporting.Query.Export;
using Ticketing.EndPoints.Reporting.Query.GetReport;

namespace Ticketing.EndPoints.Reporting.Query.DownloadReport
{
    public class GetReportHandler
    {
        public class Handler(ITicketService ticketService,
                            IProjectService projectService,
                            IStatusService statusService,
                            IExport _export,
                            ILogger<GetReportHandler> _logger) :
                            IRequestHandler<GetReportQuery, FileContentResult>
        {
            private string statusRturn(int a)
            {
                switch (a)
                {
                    case 1:
                        return "انجام شده";
                    case 2:
                        return "جدید";
                    case 3:
                        return "ارجاع به یرا";
                    case 4:
                        return "رد شده";
                    case 5:
                        return "بازگشت از ویرا";
                    case 6:
                        return "انجام شده در انتظار تایید";
                    case 7:
                        return "در صف انجام پردازش";
                    case 8:
                        return "درحال انجام";
                    case 9:
                        return "رد شده در انتظار تایید";
                }
                return "تعریف نشده";
            }
            private string projectRturn(int a)
            {
                switch (a)
                {
                    case 1:
                        return "سامانه مدیریت پرونده ها";
                    case 2:
                        return "سامانه میز خدمت";
                    case 3:
                        return "سامانه امحا";
                    case 4:
                        return "سامانه تبادل اطلاعات";
                    case 5:
                        return "سامانه هوش تجاری";
                    case 6:
                        return "زیر ساخت";
                }
                return "تعریف نشده";
            }
            private string requestRturn(object a)
            {
                switch (a)
                {
                    case RequestType.Support:
                        return "پشتیبانی";
                    case RequestType.Develop:
                        return "توسعه";
                }
                return "تعریف نشده";
            }
            private string priorityRturn(object a)
            {
                switch (a)
                {
                    case Priority.high:
                        return "بالا";
                    case Priority.medium:
                        return "پایین";
                    case Priority.low:
                        return "متوسط";
                }
                return "تعریف نشده";
            }
            public async Task<FileContentResult> Handle(GetReportQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = new List<Domain.Entities.Ticket>();
                    var liststatus = await statusService.ListAsync(null);
                    var listProject = await projectService.ListAsync(null);
                    var footersum = new List<string>(); // For foter sum colum (Empty in this code)
                    
                    result = await ticketService.ListAsync(a =>
                                                          (request.ProjectId == (int)ProjectId.all || a.ProjectId == request.ProjectId) &&
                                                          (request.Priority == Priority.all || a.Priority == request.Priority) &&
                                                          (request.RequestType == RequestType.all || a.RequestTypeId == request.RequestType) &&
                                                          (request.StatusId == (int)StatusId.all || a.StatusId == request.StatusId) &&
                                                          (request.DeveloperId == Developer.all || a.DeveloperId == request.DeveloperId));
                    if (request.StartDateTime != null)
                    {
                        result = result.Where(a => (
                                              request.StartDateTime == DateTime.MinValue || a.InsertDate >= request.StartDateTime)).ToList();
                    }
                    if (request.EndDateTime != null)
                    {
                        request.EndDateTime = request.EndDateTime?.AddHours(23);// Explanations in the bottom line
                        request.EndDateTime = request.EndDateTime?.AddMinutes(59); // to set the end day time to 11:59 p.m
                        result = result.Where(a => (
                                              request.EndDateTime == DateTime.MinValue || a.InsertDate <= request.EndDateTime)).ToList();
                    }

                    DateTime NowDate = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();
                    string title = string.Format("{2}-{1}-{0}", pc.GetDayOfMonth(NowDate) , pc.GetMonth(NowDate), pc.GetYear(NowDate));

                    #region Handel null start and end date
                    string start = "تایین نشده";
                    string end = "تایین نشده";
                    if (request.StartDateTime != null)
                    {
                        start = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.StartDateTime), pc.GetMonth((DateTime)request.StartDateTime), pc.GetDayOfMonth((DateTime)request.StartDateTime));
                    }
                    if(request.EndDateTime != null)
                    {
                        end = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.EndDateTime), pc.GetMonth((DateTime)request.EndDateTime), pc.GetDayOfMonth((DateTime)request.EndDateTime));
                    }
                    #endregion

                    var ReportResultHeader = new ReportHeaderInfo()
                    {
                        startDate = start,

                        endDate = end,

                        PrintDate = string.Format("{2}/{1}/{0} {3}:{4}", pc.GetYear(NowDate), pc.GetMonth(NowDate),
                        pc.GetDayOfMonth(NowDate), pc.GetHour(NowDate), pc.GetMinute(NowDate)),
                    };

                    var headerInfoxslx = new List<string>
                    {
                        "شماره ردیف",
                        "شماره تیکت",
                        "ثبت کننده",
                        "تاریخ ثبت",
                        "نوع درخواست",
                        "اولویت",
                        "عنوان",
                        "سامانه",
                        "وضعیت تیکت",
                        "انجام دهنده",
                        "ساعت صرف شده",
                        "تاریخ و ساعت تحویل تیکت"
                    };

                    var CreatedList = result.Select((x, r) => new[]
                    {
                        new CellInfo() {Text = (r + 1).ToString() },
                        new CellInfo() {Text = x.TicketNumber ,DynamicWidth=true},
                        new CellInfo() {Text = x.Username ,DynamicWidth=true},
                        new CellInfo() {Text = new PersianCalendar().GetYear(x.InsertDate).ToString("D2") + "/" + 
                                               new PersianCalendar().GetMonth(x.InsertDate).ToString("D2") + "/" + 
                                               new PersianCalendar().GetDayOfMonth(x.InsertDate).ToString("D2") 
                                               ,DynamicWidth=true},
                        new CellInfo() {Text = requestRturn(x.RequestTypeId) ,DynamicWidth=true},
                        new CellInfo() {Text = priorityRturn(x.Priority) ,DynamicWidth=true},
                        new CellInfo() {Text = x.Title ,DynamicWidth=true},
                        new CellInfo() {Text = projectRturn(x.ProjectId) ,DynamicWidth=true}, //x.ProjectId.ToString()
                        new CellInfo() {Text = statusRturn(x.StatusId) ,DynamicWidth=true},
                        new CellInfo() {Text = x.DeveloperId.ToString() ,DynamicWidth=true},
                        new CellInfo() {Text = (x.TicketTime==null?"0":x.TicketTime) ,DynamicWidth=true},
                        new CellInfo() {Text = x.ProcessEndDateTime == null?"ثبت نشده":
                                               new PersianCalendar().GetYear((DateTime)x.ProcessEndDateTime).ToString("D2") + "/" +
                                               new PersianCalendar().GetMonth((DateTime)x.ProcessEndDateTime).ToString("D2") + "/" + 
                                               new PersianCalendar().GetDayOfMonth((DateTime)x.ProcessEndDateTime).ToString("D2") + "  " + 
                                               new PersianCalendar().GetHour((DateTime)x.ProcessEndDateTime).ToString("D2") + ":" +
                                               new PersianCalendar().GetMinute((DateTime)x.ProcessEndDateTime).ToString("D2")
                                               ,DynamicWidth=true},
                    }).ToList();

                    return _export.ToExcel(title, headerInfoxslx, new List<ExcelDataType>
                    {
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                        ExcelDataType.Text,
                    }, CreatedList, footersum, headerInfo: new List<string>
                    {
                        "از تاریخ :" + ReportResultHeader.startDate,
                        "تا تاریخ :" + ReportResultHeader.endDate,
                        "تاریخ چاپ گزارش :" + ReportResultHeader.PrintDate,
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Get Error : DownloadFile- Handle " + ex.Message);
                }

                return null;
            }
        }
    }
}
