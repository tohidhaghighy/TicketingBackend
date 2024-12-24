namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public partial class GetDashboardChartDataHandler
    {
        #region DevelopResultYear class
        public class DevelopResultYear
        {
            public string Month { get; set; }
            public int Total { get; set; } // For developInRFPTotal and developOutRFPTotal
            public int Done { get; set; } // For developInRFPDone and developOutRFPDone
        }
        #endregion
    }
}
