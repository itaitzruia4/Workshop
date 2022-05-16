namespace API.Requests
{
    public class ProductSearchRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public string KeyWords { get; set; }
        public string Category { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int ProductReview { get; set; }
    }
}
