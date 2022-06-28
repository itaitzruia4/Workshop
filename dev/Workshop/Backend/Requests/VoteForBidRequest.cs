namespace API.Requests
{
    public class VoteForBidRequest : BidRequest
    {
        public bool ToAccept { get; set; }
    }
}
