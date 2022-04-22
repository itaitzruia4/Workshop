using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.UserPackage
{
    public class ReviewController : IReviewController
    {
        Dictionary<int, Dictionary<string, Review>> productReviews;
        Dictionary<string, Dictionary<int, Review>> userReviews;

        public ReviewController()
        {
            productReviews = new Dictionary<int, Dictionary<string, Review>>();
            userReviews = new Dictionary<string, Dictionary<int, Review>>();
        }

        public void AddReview(string user, int productId, string review)
        {
            Review toAdd = new Review(user, productId, review);
            AddToProductReviews(productId, user, toAdd);
            AddToUserReviews(user, productId, toAdd);
        }

        private void AddToProductReviews(int productId, string user, Review review)
        {
            if (!productReviews.ContainsKey(productId))
            {
                productReviews.Add(productId, new Dictionary<string, Review>());
            }
            Dictionary<string, Review> temp = productReviews[productId];
            if (!temp.ContainsKey(user))
            {
                temp.Add(user, review);
            }
            else
            {
                temp[user] = Review;
            }
        }
        private void AddToUserReviews(string user, int productId, Review review)
        {
            if (!userReviews.ContainsKey(user))
            {
                userReviews.Add(user, new Dictionary<int, List<Review>>());
            }
            Dictionary<int, List<Review>> temp = userReviews[user];
            if (!temp.ContainsKey(productId))
            {
                temp.Add(productId, review);
            }
            else
            {
                temp[productId] = Review;
            }
        }
    }
}
