using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Reporting.Query.GetReport
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
}
