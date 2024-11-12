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
                    request.EndDateTime = request.EndDateTime.AddHours(23);// Explanations in the bottom line
                    request.EndDateTime = request.EndDateTime.AddMinutes(59); // to set the end day time to 11:59 p.m

                    result = await ticketService.ListAsync(a =>
                                                          (request.ProjectId == (int)ProjectId.all || a.ProjectId == request.ProjectId) &&
                                                          (request.Priority == Priority.all || a.Priority == request.Priority) &&
                                                          (request.RequestType == RequestType.all || a.RequestTypeId == request.RequestType) &&
                                                          (request.StatusId == (int)StatusId.all || a.StatusId == request.StatusId) &&
                                                          (request.DeveloperId == Developer.all || a.DeveloperId == request.DeveloperId) &&
                                                          (a.InsertDate >= request.StartDateTime && a.InsertDate <= request.EndDateTime));

                    DateTime NowDate = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();
                    string title = string.Format("{2}-{1}-{0}", pc.GetDayOfMonth(NowDate) , pc.GetMonth(NowDate), pc.GetYear(NowDate));

                    var ReportResultHeader = new ReportHeaderInfo()
                    {
                        startDate = string.Format("{2}/{1}/{0}", pc.GetYear(request.StartDateTime), pc.GetMonth(request.StartDateTime), pc.GetDayOfMonth(request.StartDateTime)),

                        endDate = string.Format("{2}/{1}/{0}", pc.GetYear(request.EndDateTime) , pc.GetMonth(request.EndDateTime), pc.GetDayOfMonth(request.EndDateTime)),

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
                    };

                    var CreatedList = result.Select((x, r) => new[]
                    {
                        new CellInfo() {Text = (r + 1).ToString() },
                        new CellInfo() {Text = x.TicketNumber ,DynamicWidth=false},
                        new CellInfo() {Text = x.Username ,DynamicWidth=false},
                        new CellInfo() {Text = new PersianCalendar().GetYear(x.InsertDate) + "/" + new PersianCalendar().GetMonth(x.InsertDate).ToString("D2") + "/" + new PersianCalendar().GetDayOfMonth(x.InsertDate).ToString("D2") ,DynamicWidth=false},
                        new CellInfo() {Text = requestRturn(x.RequestTypeId) ,DynamicWidth=false},
                        new CellInfo() {Text = priorityRturn(x.Priority) ,DynamicWidth=false},
                        new CellInfo() {Text = x.Title ,DynamicWidth=false},
                        new CellInfo() {Text = projectRturn(x.ProjectId) ,DynamicWidth=false}, //x.ProjectId.ToString()
                        new CellInfo() {Text = statusRturn(x.StatusId) ,DynamicWidth=false},
                        new CellInfo() {Text = x.DeveloperId.ToString() ,DynamicWidth=false},
                        new CellInfo() {Text = (x.TicketTime==null?"0":x.TicketTime) ,DynamicWidth=false}
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
