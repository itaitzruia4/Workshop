namespace API.Requests
{
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
    }
}
