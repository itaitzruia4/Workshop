namespace API.Requests
{
    public class DailyStatisticsRequest : MemberRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
