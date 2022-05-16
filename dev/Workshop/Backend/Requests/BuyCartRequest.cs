namespace API.Requests
{
    public class BuyCartRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public string Address { get; set; }
    }
}
