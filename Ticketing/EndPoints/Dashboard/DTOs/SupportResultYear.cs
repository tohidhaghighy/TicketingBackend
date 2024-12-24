namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public partial class GetDashboardChartDataHandler
    {
        #region DevelopResultYear class
        #endregion

        #region SupportResultYear class
        public class SupportResultYear
        {
            public string Month { get; set; }
            public int SupportTime { get; set; } // For support year result
            public int CompanyCommitmentTime { get; set; } // For support year result
        }
        #endregion
    }
}
