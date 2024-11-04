using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Reporting.Query.Export;

namespace Ticketing.EndPoints.Reporting.Query.DownloadReport
{
    public class GetReportQuery : IRequest<FileContentResult>
    {
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public Priority Priority { get; set; }
        public RequestType RequestType { get; set; }
        public int StatusId { get; set; }
        public Developer DeveloperId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
    public class ReportHeaderInfo
    {
        public string Title { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string PrintDate { get; set; }
    }
    public class GetReport
    {
        public class Handler(ITicketService ticketService,
                            IProjectService projectService,
                            IStatusService statusService,
                            IExport _export,
                            ILogger<GetReport> _logger) :
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
                                                          (a.InsertDate >= request.StartDateTime || a.CloseDate >= request.StartDateTime) &&
                                                          (a.InsertDate <= request.EndDateTime || a.CloseDate <= request.EndDateTime));

                    DateTime NowDate = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();

                    var ReportResultHeader = new ReportHeaderInfo()
                    {
                        Title = request.Title,
                        startDate = string.Format("{2}/{1}/{0}", request.StartDateTime.Year, request.StartDateTime.Month,
                        request.StartDateTime.Day),
                        endDate = string.Format("{2}/{1}/{0}", request.EndDateTime.Year, request.EndDateTime.Month,
                        request.EndDateTime.Day),
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

                    return _export.ToExcel(request.Title, headerInfoxslx, new List<ExcelDataType>
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
                        "عنوان : " + ReportResultHeader.Title,
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
    public class DownloadReportModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "api/v1/downloadReport",
                    async (IMediator mediator, [AsParameters] GetReportQuery query,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);
                        return Results.File(result.FileContents, result.ContentType, result.FileDownloadName);
                    })
                .WithOpenApi()
                .WithTags("Report")
                .Produces<object[]>();
        }
    }
}
