namespace API.Requests
{
    public class ProductReviewRequest : ProductRequest
    {
        public string Review { get; set; }
        public int Rating { get; set; }
    }
}
