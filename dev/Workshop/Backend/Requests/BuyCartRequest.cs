namespace API.Requests
{
    public class BuyCartRequest : MemberRequest
    {
        public string Address { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
