using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Status.Query.GetStatus;

public class GetStatusListHandler
{
    public class Handler
    (IStatusService statusService,
        ILogger<GetStatusListHandler> _logger) : IRequestHandler<
        GetStatusListQuery, object>
    {

        public async Task<object> Handle(GetStatusListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await statusService.ListAsync(null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : GetGroupList- Handle " + ex.Message);
            }

            return null;
        }
    }
}