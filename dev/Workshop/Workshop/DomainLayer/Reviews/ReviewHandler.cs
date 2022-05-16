using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.Reviews
{
    public class ReviewHandler : IReviewHandler
    {
        ConcurrentDictionary<int, ConcurrentDictionary<string, ReviewDTO>> productReviews;
        ConcurrentDictionary<string, ConcurrentDictionary<int, ReviewDTO>> userReviews;

        public ReviewHandler()
        {
            productReviews = new ConcurrentDictionary<int, ConcurrentDictionary<string, ReviewDTO>>();
            userReviews = new ConcurrentDictionary<string, ConcurrentDictionary<int, ReviewDTO>>();
        }

        public ReviewDTO AddReview(string user, int productId, string review, int rating)
        {
            ReviewDTO reviewDTO = new ReviewDTO(user, productId, review, rating);
            AddToProductReviews(productId, user, reviewDTO);
            AddToUserReviews(user, productId, reviewDTO);
            return reviewDTO;
        }

        private void AddToProductReviews(int productId, string user, ReviewDTO review)
        {
            productReviews.TryAdd(productId, new ConcurrentDictionary<string, ReviewDTO>());
            ConcurrentDictionary<string, ReviewDTO> temp;
            productReviews.TryGetValue(productId, out temp);
            if (!temp.TryAdd(user, review))
            {
                ReviewDTO oldReview;
                temp.TryGetValue(user, out oldReview);
                temp.TryUpdate(user, review, oldReview);
            }
        }
        private void AddToUserReviews(string user, int productId, ReviewDTO review)
        {
            userReviews.TryAdd(user, new ConcurrentDictionary<int, ReviewDTO>());
            ConcurrentDictionary<int, ReviewDTO> temp;
            userReviews.TryGetValue(user, out temp);
            if (!temp.TryAdd(productId, review))
            {
                ReviewDTO oldReview;
                temp.TryGetValue(productId, out oldReview);
                temp.TryUpdate(productId, review, oldReview);
            }
        }

        public double GetProductRating(int productId)
        {
            ConcurrentDictionary<string, ReviewDTO> reviews;
            if (productReviews.TryGetValue(productId, out reviews))
            {
                return Queryable.Average(reviews.Values.Select(r => r.Rating).AsQueryable());
            }
            return -1;
        }
    }
}