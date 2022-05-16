using Workshop.DomainLayer.Reviews;

namespace API.Responses
{
    public class ReviewResponse
    {
        public ReviewDTO Review;
        public string Error;

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
