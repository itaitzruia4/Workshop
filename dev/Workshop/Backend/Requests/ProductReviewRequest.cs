namespace API.Requests
{
    public class ProductReviewRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public int ProductId { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
    }
}
