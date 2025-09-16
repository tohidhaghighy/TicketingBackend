using MediatR;
using Ticketing.Domain.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Ticket.Command.AddTicket;

public class AddTicketHandler
{
    public class Handler(
        ITicketService ticketService, 
        IProjectService projectService, 
        ITicketFlowService ticketFlowService, 
        IProjectRoleService projectRoleService, 
        ILogger<AddTicketHandler> _logger)
        : IRequestHandler<AddTicketQuery, object>
    {
        public async Task<object> Handle(AddTicketQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string fileName = "";
                if (request.File != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    FileInfo fileInfo = new FileInfo(request.File.FileName);
                    if (!fileInfo.Extension.Contains("exe"))
                    {
                        fileName = Guid.NewGuid() + fileInfo.Extension;
                        string fileNameWithPath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            request.File.CopyTo(stream);
                        }
                    }
                }
                var persiandate = new System.Globalization.PersianCalendar();
                var getlastticket = await ticketService.ListAsync(null);
                var rowNumber = (getlastticket.Count() == 0 ? 1 : getlastticket.Last().TicketRowNumber + 1);
                if (request.RequestType == RequestType.Support)
                {
                    request.IsSchedule = IsSchedule.Support;
                }

                #region Find project RoleId
                var relatedRoleId = await projectRoleService.GetAsync(a => a.ProjectId == request.ProjectId && a.RoleId != (int)Role.admindir && a.RoleId != (int)Role.normalUser);
                #endregion

                var result = await ticketService.AddAsync(new Domain.Entities.Ticket()
                {
                    CurrentRoleId = relatedRoleId.RoleId,
                    InsertedRoleId = request.RoleId,
                    Text = request.Text,
                    Title = request.Title,
                    Priority = request.Priority,
                    UserId = request.UserId,
                    RequestTypeId = request.RequestType,
                    StatusId = (int)StatusId.inserted,
                    CloseDate = DateTime.Now,
                    InsertDate = DateTime.Now,
                    ProjectId = request.ProjectId,
                    FilePath = fileName,
                    Username = request.Username,
                    LastChangeDatetime = null,
                    TicketRowNumber = rowNumber,
                    TicketNumber = persiandate.GetYear(DateTime.Now).ToString() +
                               persiandate.GetMonth(DateTime.Now).ToString() +
                               persiandate.GetDayOfMonth(DateTime.Now).ToString() +
                               rowNumber.Value.ToString("000#"),
                    AssignUserId = 0,
                    IsSchedule = request.IsSchedule.Value,
                    TicketTime = "0"
                });

                #region Add first TicketFlow For ticket creator
                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = request.RoleId,
                    InsertDate = DateTime.Now,
                    StatusId = (int)StatusId.inserted,
                    UserId = request.UserId,//سازنده تیکت
                    Username = "ایجاد شده توسط" + " " + request.Username,//نام سازنده تیکت
                    TicketId = result.Id,
                    PreviousRoleId = request.RoleId
                });
                #endregion

                #region Add second TicketFlow for AssignedRole
                
                await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                {
                    CurrentRoleId = relatedRoleId.RoleId,
                    InsertDate = DateTime.Now,
                    StatusId = (int)StatusId.inserted,
                    UserId = request.UserId,
                    Username = FindAssignedRoleName(relatedRoleId.RoleId),//ارجاع شده به
                    TicketId = result.Id,
                    PreviousRoleId = request.RoleId
                });

                #region Find assigned role name
                
                #endregion

                #endregion
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : AddTicketHandler- Handle " + ex.Message);
            }

            return null;
        }
        public string FindAssignedRoleName(int relatedRoleId)
        {
            var assignRoleName = string.Empty;
            switch (relatedRoleId)
            {
                case (3):
                    assignRoleName = "ارجاع به معاونت آمار";
                    break;
                case (4):
                    assignRoleName = "ارجاع به معاونت زیرساخت، شبکه و امنیت";
                    break;
                case (5):
                    assignRoleName = "ارجاع به معاونت فناوری اطلاعات";
                    break;
            }
            return assignRoleName;
        }
    }
}