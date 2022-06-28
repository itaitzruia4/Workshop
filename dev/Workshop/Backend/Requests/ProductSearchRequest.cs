namespace API.Requests
{
    public class ProductSearchRequest : MemberRequest
    {
        public string Keywords { get; set; }
        public string Category { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double ProductReview { get; set; }
    }
}
