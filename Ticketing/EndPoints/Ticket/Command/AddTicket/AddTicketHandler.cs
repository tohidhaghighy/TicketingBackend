using MediatR;
using Ticketing.Domain.Contracts;

namespace Ticketing.EndPoints.Ticket.Command.AddTicket;

public class AddTicketHandler
{
    public class Handler(ITicketService ticketService, IProjectService projectService,ITicketFlowService ticketFlowService, ILogger<AddTicketHandler> _logger)
        : IRequestHandler<AddTicketQuery, object>
    {
        public async Task<object> Handle(AddTicketQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string fileName = "";
                if (request.File!=null)
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
                var rowNumber= (getlastticket.Count() == 0 ? 1 : getlastticket.Last().TicketRowNumber + 1);

                if (request.RoleId == 4) //if admin tazirat add new ticket, automaticly new ticket send to vira admin and dont need manually chang status in new tickets
                {
                    var result = await ticketService.AddAsync(new Domain.Entities.Ticket()
                    {
                        CurrentRoleId = 5,//admin vira
                        InsertedRoleId = request.RoleId,
                        Text = request.Text,
                        Title = request.Title,
                        Priority = request.Priority,
                        UserId = request.UserId,
                        RequestTypeId = request.RequestType,
                        StatusId = 3,// Send to vira
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
                                   rowNumber.Value.ToString("000#")
                    });

                    await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                    {
                        CurrentRoleId = 5,
                        InsertDate = DateTime.Now,
                        StatusId = 3,
                        UserId = request.UserId,
                        Username = request.Username,
                        TicketId = result.Id,
                        PreviousRoleId = request.RoleId
                    });
                    return result;
                }
                else
                {
                    var result = await ticketService.AddAsync(new Domain.Entities.Ticket()
                    {
                        CurrentRoleId = 4,//admin taz
                        InsertedRoleId = request.RoleId,
                        Text = request.Text,
                        Title = request.Title,
                        Priority = request.Priority,
                        UserId = request.UserId,
                        RequestTypeId = request.RequestType,
                        StatusId = 2,// new ticket
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
                                   rowNumber.Value.ToString("000#")
                    });

                    await ticketFlowService.AddAsync(new Domain.Entities.TicketFlow()
                    {
                        CurrentRoleId = 4,
                        InsertDate = DateTime.Now,
                        StatusId = 2,
                        UserId = request.UserId,
                        Username = request.Username,
                        TicketId = result.Id,
                        PreviousRoleId = request.RoleId
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Error : AddTicketHandler- Handle " + ex.Message);
            }

            return null;
        }
    }
}