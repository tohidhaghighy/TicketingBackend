using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Ticketing.Domain.Contracts;
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
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string PrintDate { get; set; }
        public string Title { get; set; }
    }
    public class PreTableHeader
    {
        public string? TicketRowNumber { get; set; }
        public string? TicketNumber { get; set; }
        public string? Username { get; set; }
        public string? Date { get; set; }
        public string? RequestType { get; set; }
        public string? Priority { get; set; }
        public string? Title { get; set; }
        public string? Project { get; set; }
        public string? StatusId { get; set; }
        public string? DeveloperId { get; set; }
        public string? TicketTime { get; set; }
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
            public async Task<FileContentResult> Handle(GetReportQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = new List<Domain.Entities.Ticket>();
                    var liststatus = await statusService.ListAsync(null);
                    var listProject = await projectService.ListAsync(null);
                    var footersum = new List<string>();

                    if (request.ProjectId == 0 &&
                        request.Priority == Priority.all &&
                        request.RequestType == RequestType.all &&
                        request.StatusId == 0 &&
                        request.DeveloperId == Developer.all)
                    {
                        result = await ticketService.ListAsync(a => a.InsertDate >= request.StartDateTime
                                                                 || a.CloseDate > request.StartDateTime &&
                                                                    a.InsertDate <= request.EndDateTime
                                                                 || a.CloseDate <= request.EndDateTime);
                    }
                    else if (request.Priority == Priority.all &&
                             request.RequestType == RequestType.all &&
                             request.StatusId == 0 &&
                             request.DeveloperId == Developer.all)
                    {
                        result = await ticketService.ListAsync(a => a.ProjectId == request.ProjectId &&
                                                               a.InsertDate >= request.StartDateTime
                                                               || a.CloseDate > request.StartDateTime &&
                                                               a.InsertDate <= request.EndDateTime
                                                               || a.CloseDate <= request.EndDateTime);
                    }
                    else if (request.RequestType == RequestType.all &&
                             request.StatusId == 0 &&
                             request.DeveloperId == Developer.all)
                    {
                        result = await ticketService.ListAsync(a => a.ProjectId == request.ProjectId &&
                                                                    a.Priority == request.Priority &&
                                                                    a.InsertDate >= request.StartDateTime
                                                                    || a.CloseDate > request.StartDateTime &&
                                                                    a.InsertDate <= request.EndDateTime
                                                                    || a.CloseDate <= request.EndDateTime);
                    }
                    else if (request.StatusId == 0 &&
                             request.DeveloperId == Developer.all)
                    {
                        result = await ticketService.ListAsync(a => a.ProjectId == request.ProjectId &&
                                                                    a.Priority == request.Priority &&
                                                                    a.RequestTypeId == request.RequestType &&
                                                                    a.InsertDate >= request.StartDateTime
                                                                    || a.CloseDate > request.StartDateTime &&
                                                                    a.InsertDate <= request.EndDateTime
                                                                    || a.CloseDate <= request.EndDateTime);
                    }
                    else if (request.DeveloperId == Developer.all)
                    {
                        result = await ticketService.ListAsync(a => a.ProjectId == request.ProjectId &&
                                                                    a.Priority == request.Priority &&
                                                                    a.RequestTypeId == request.RequestType &&
                                                                    a.StatusId == request.StatusId &&
                                                                    a.InsertDate >= request.StartDateTime
                                                                    || a.CloseDate > request.StartDateTime &&
                                                                    a.InsertDate <= request.EndDateTime
                                                                    || a.CloseDate <= request.EndDateTime);
                    }
                    else
                    {
                        result = await ticketService.ListAsync(a => a.ProjectId == request.ProjectId &&
                                                                    a.Priority == request.Priority &&
                                                                    a.RequestTypeId == request.RequestType &&
                                                                    a.StatusId == request.StatusId &&
                                                                    a.DeveloperId == request.DeveloperId &&
                                                                    a.InsertDate >= request.StartDateTime
                                                                    || a.CloseDate > request.StartDateTime &&
                                                                    a.InsertDate <= request.EndDateTime
                                                                    || a.CloseDate <= request.EndDateTime);
                    }

                    DateTime NowDate = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();

                    var ReportResultHeader = new ReportHeaderInfo()
                    {
                        Title = request.Title,
                        startDate = request.StartDateTime.ToLongDateString(),
                        endDate = request.EndDateTime.ToLongDateString(),
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
                        new CellInfo { Text = (r + 1).ToString() },
                        new CellInfo() {Text = x.TicketRowNumber.ToString(),DynamicWidth=true },
                        new CellInfo() {Text = x.TicketNumber ,DynamicWidth=true},
                        new CellInfo(){Text = x.Username ,DynamicWidth=true},
                        new CellInfo() { Text= x.InsertDate.ToLongDateString() ,DynamicWidth=true},
                        new CellInfo() {Text = x.RequestTypeId.ToString() ,DynamicWidth=true},
                        new CellInfo() { Text = x.Priority.ToString() ,DynamicWidth=true},
                        new CellInfo() { Text = x.Title ,DynamicWidth=true},
                        new CellInfo() { Text = x.ProjectId.ToString() ,DynamicWidth=true},
                        new CellInfo() { Text = x.StatusId.ToString() ,DynamicWidth=true},
                        new CellInfo() { Text = x.DeveloperId.ToString() ,DynamicWidth=true},
                        new CellInfo() { Text = x.TicketTime ,DynamicWidth=true}
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
                        "عنوان" + ReportResultHeader.Title,
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
