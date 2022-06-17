namespace API.Requests
{
    public class StoreCreationRequest : MemberRequest
    {
        public string StoreName { get; set; }
        public string Date { get; set; }
    }
}
