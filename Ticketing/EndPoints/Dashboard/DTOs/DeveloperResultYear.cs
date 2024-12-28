namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public partial class GetDashboardChartDataHandler
    {
        #region SupportResultYear class
        #endregion

        #region DeveloperResultYear class
        public class DeveloperResultYear
        {
            public string Month { get; set; }
            public List<DeveloperData> DeveloperCounts { get; set; } = new(); // Developer ticket counts
            public List<DeveloperData> DeveloperTimes { get; set; } = new();  // Developer ticket times
        }
        #endregion
    }
}
