using MediatR;
using System.Data;
using Ticketing.Domain.Enums;

namespace Ticketing.EndPoints.Search.Query.GetSearchResult
{
    public class GetSearchResultQuery : IRequest<object>
    {
        public string TicketNumber { get; set; } = "";  //شماره تیکت null
        public string Title { get; set; } = "";  //متن تیکت null
        public List<int> InsertedRoleId { get; set; }  //رول شخص ثبت کننده Role.all == 0
        public string Username { get; set; } = "";  //اسم شخص ثبت کننده
        public List<int> CurrentRoleId { get; set; }  //تیکت الان دست کدوم رول هستش
        public List<int> StatusId { get; set; }  //وضعیت تیکت
        public List<int> ProjectId { get; set; }  //آیدی سامانه
        public List<int> RequestType { get; set; }  //نوع درخواست
        public List<int> DeveloperId { get; set; }  //انجام دهنده
        public DateTime? InsertStartDateTime { get; set; }
        public DateTime? InsertEndDateTime { get; set; }
        public DateTime? CloseStartDateTime { get; set; }
        public DateTime? CloseEndDateTime { get; set; }
    }
}
