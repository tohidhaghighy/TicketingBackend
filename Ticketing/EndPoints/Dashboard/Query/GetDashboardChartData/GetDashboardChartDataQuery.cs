using MediatR;

namespace Ticketing.EndPoints.Dashboard.Query.GetDashboardChartData
{
    public class GetDashboardChartDataQuery : IRequest<object>
    {
        public int monthId { set; get; }
    }
}
