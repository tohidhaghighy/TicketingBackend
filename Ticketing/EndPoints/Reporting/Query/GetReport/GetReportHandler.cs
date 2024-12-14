using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;
using Ticketing.EndPoints.Reporting.Query.Dtos;
using Ticketing.EndPoints.Reporting.Query.Export;
using Ticketing.EndPoints.Reporting.Query.GetReport;
using Ticketing.Utility;

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
            #region enum returns
            private string statusRturn(int statusId)
            {
                switch (statusId)
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
            private string projectRturn(int projectId)
            {
                switch (projectId)
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
            private string requestRturn(object requestId)
            {
                switch (requestId)
                {
                    case RequestType.Support:
                        return "پشتیبانی";
                    case RequestType.Develop:
                        return "توسعه";
                }
                return "تعریف نشده";
            }
            private string priorityRturn(object priorityId)
            {
                switch (priorityId)
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
            private string IsScheduleReturn(object isScheduleId)
            {
                switch (isScheduleId)
                {
                    case IsSchedule.yes:
                        return "بله";
                    case IsSchedule.no:
                        return "خیر";
                    case IsSchedule.Support:
                        return "-";
                }
                return "تعریف نشده";
            }
            #endregion

            public async Task<FileContentResult> Handle(GetReportQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    #region Props
                    var result = new List<Domain.Entities.Ticket>();
                    var liststatus = await statusService.ListAsync(null);
                    var listProject = await projectService.ListAsync(null);
                    var footersum = new List<string>(); // For foter sum colum (Empty in this code)
                    #endregion

                    #region Multi selected items
                    var StatusId = request.StatusId.IsNullOrEmpty() ? null : request.StatusId.ConvertStringToListIntiger();
                    var ProjectId = request.ProjectId.IsNullOrEmpty() ? null : request.ProjectId.ConvertStringToListIntiger();
                    var RequestType = request.RequestType.IsNullOrEmpty() ? null : request.RequestType.ConvertStringToListIntiger();
                    var DeveloperId = request.DeveloperId.IsNullOrEmpty() ? null : request.DeveloperId.ConvertStringToListIntiger();
                    var Priority = request.Priority.IsNullOrEmpty() ? null : request.Priority.ConvertStringToListIntiger();
                    var IsSchadule = request.IsSchadule.IsNullOrEmpty() ? null : request.IsSchadule.ConvertStringToListIntiger();
                    #endregion

                    #region Handel End DateTime
                    request.InsertEndDateTime = request.InsertEndDateTime == DateTime.MinValue || request.InsertEndDateTime == null ? null : request.InsertEndDateTime?.AddHours(23);
                    request.InsertEndDateTime = request.InsertEndDateTime == DateTime.MinValue || request.InsertEndDateTime == null ? null : request.InsertEndDateTime?.AddMinutes(59);

                    request.CloseEndDateTime = request.CloseEndDateTime == DateTime.MinValue || request.CloseEndDateTime == null ? null : request.CloseEndDateTime?.AddHours(23);
                    request.CloseEndDateTime = request.CloseEndDateTime == DateTime.MinValue || request.CloseEndDateTime == null ? null : request.CloseEndDateTime?.AddMinutes(59);
                    #endregion

                    #region find data
                    result = await ticketService.ListAsync(a =>
                                                          (request.ProjectId.IsNullOrEmpty() || ProjectId.Contains(a.ProjectId)) &&
                                                          (request.Priority.IsNullOrEmpty() || Priority.Contains((int)a.Priority)) &&
                                                          (request.RequestType.IsNullOrEmpty() || RequestType.Contains((int)a.RequestTypeId)) &&
                                                          (request.StatusId.IsNullOrEmpty() || StatusId.Contains(a.StatusId)) &&
                                                          (request.DeveloperId.IsNullOrEmpty() || DeveloperId.Contains((int)a.DeveloperId)) &&
                                                          (request.IsSchadule.IsNullOrEmpty() || IsSchadule.Contains((int)a.IsSchedule)) &&
                                                          (!request.InsertStartDateTime.HasValue || a.InsertDate >= request.InsertStartDateTime) &&
                                                          (!request.InsertEndDateTime.HasValue || a.InsertDate <= request.InsertEndDateTime) &&
                                                          (!request.CloseStartDateTime.HasValue || a.ProcessEndDateTime >= request.CloseStartDateTime) &&
                                                          (!request.CloseEndDateTime.HasValue || a.ProcessEndDateTime <= request.CloseEndDateTime));

                    result = result.OrderBy(a => a.InsertDate).ToList(); //sort by InsertDate
                    #endregion

                    #region time handeling time
                    DateTime NowDate = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();
                    string title = string.Format("{2}-{1}-{0}", pc.GetDayOfMonth(NowDate), pc.GetMonth(NowDate), pc.GetYear(NowDate));
                    #endregion

                    #region Handel  start and end date
                    string I_start = "تعیین نشده";
                    string I_end = "تعیین نشده";
                    string C_start = "تعیین نشده";
                    string C_end = "تعیین نشده";
                    if (request.InsertStartDateTime != null)
                    {
                        I_start = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.InsertStartDateTime), pc.GetMonth((DateTime)request.InsertStartDateTime), pc.GetDayOfMonth((DateTime)request.InsertStartDateTime));
                    }
                    if (request.InsertEndDateTime != null)
                    {
                        I_end = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.InsertEndDateTime), pc.GetMonth((DateTime)request.InsertEndDateTime), pc.GetDayOfMonth((DateTime)request.InsertEndDateTime));
                    }
                    if (request.CloseStartDateTime != null)
                    {
                        C_start = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.CloseStartDateTime), pc.GetMonth((DateTime)request.CloseStartDateTime), pc.GetDayOfMonth((DateTime)request.CloseStartDateTime));
                    }
                    if (request.CloseEndDateTime != null)
                    {
                        C_end = string.Format("{2}/{1}/{0}", pc.GetYear((DateTime)request.CloseEndDateTime), pc.GetMonth((DateTime)request.CloseEndDateTime), pc.GetDayOfMonth((DateTime)request.CloseEndDateTime));
                    }
                    #endregion

                    #region ReportResultHeader
                    var ReportResultHeader = new ReportHeaderInfo()
                    {
                        InsertStartDateTime = I_start,

                        InsertEndDateTime = I_end,

                        CloseStartDateTime = C_start,

                        CloseEndDateTime = C_end,

                        PrintDate = string.Format("{2}/{1}/{0} {3}:{4}", pc.GetYear(NowDate), pc.GetMonth(NowDate),
                        pc.GetDayOfMonth(NowDate), pc.GetHour(NowDate), pc.GetMinute(NowDate)),
                    };
                    #endregion

                    #region headerInfoxslx
                    var headerInfoxslx = new List<string>
                    {
                        "شماره ردیف",
                        "شماره تیکت",
                        "ثبت کننده",
                        "تاریخ ثبت",
                        "نوع درخواست",
                        "بر اساس برنامه زمانبندی می باشد",
                        "اولویت",
                        "عنوان",
                        "سامانه",
                        "وضعیت تیکت",
                        "انجام دهنده",
                        "ساعت صرف شده",
                        "تاریخ و ساعت تحویل تیکت"
                    };
                    #endregion

                    #region cellInfo
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
                        new CellInfo() {Text = IsScheduleReturn(x.IsSchedule) , DynamicWidth=true},
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
                    #endregion

                    #region return excel
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
                        "از تاریخ ایجاد :" + ReportResultHeader.InsertStartDateTime,
                        "تا تاریخ ایجاد :" + ReportResultHeader.InsertEndDateTime,
                        "از تاریخ تحویل :" + ReportResultHeader.CloseStartDateTime,
                        "تا تاریخ تحویل :" + ReportResultHeader.CloseEndDateTime,
                        "تاریخ چاپ گزارش :" + ReportResultHeader.PrintDate,
                    });
                    #endregion
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
