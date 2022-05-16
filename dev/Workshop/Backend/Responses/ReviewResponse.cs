using Workshop.DomainLayer.Reviews;

namespace API.Responses
{
    public class ReviewResponse
    {
        public ReviewDTO Review { get; set; }
        public string Error { get; set; }

        public ReviewResponse(ReviewDTO rev)
        {
            Review = rev;
        }
        public ReviewResponse(string error)
        {
            Error = error;
        }
    }
}
