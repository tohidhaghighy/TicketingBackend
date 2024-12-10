namespace Ticketing.EndPoints.Reporting.Query.Dtos
{
    public class ReportHeaderInfo
    {
        public string InsertStartDateTime { get; set; }
        public string InsertEndDateTime { get; set; }
        public string CloseStartDateTime { get; set; }
        public string CloseEndDateTime { get; set; }
        public string PrintDate { get; set; }
    }
}
