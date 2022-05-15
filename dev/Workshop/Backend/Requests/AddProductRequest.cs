namespace API.Requests
{
    public class AddProductRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }
}
