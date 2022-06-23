namespace API.Requests
{
    public class BuyBidRequest : BuyCartRequest
    {
        public string Membername { get; set; }
        public int StoreId { get; set; }
        public int BidId { get; set; }

    }
}
