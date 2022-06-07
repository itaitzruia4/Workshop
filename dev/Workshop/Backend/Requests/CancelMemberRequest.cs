namespace API.Requests
{
    public class CancelMemberRequest : MemberRequest
    {
        public string MemberToCancel { get; set; }
    }
}
