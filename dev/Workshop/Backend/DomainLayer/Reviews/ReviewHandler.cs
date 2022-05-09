using System;
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
        Dictionary<int, Dictionary<string, ReviewDTO>> productReviews;
        Dictionary<string, Dictionary<int, ReviewDTO>> userReviews;

        public ReviewHandler()
        {
            productReviews = new Dictionary<int, Dictionary<string, ReviewDTO>>();
            userReviews = new Dictionary<string, Dictionary<int, ReviewDTO>>();
        }

        public ReviewDTO AddReview(string user, int productId, string review)
        {
            ReviewDTO reviewDTO = new ReviewDTO(user, productId, review);
            AddToProductReviews(productId, user, reviewDTO);
            AddToUserReviews(user, productId, reviewDTO);
            return reviewDTO;
        }

        private void AddToProductReviews(int productId, string user, ReviewDTO review)
        {
            if (!productReviews.ContainsKey(productId))
            {
                productReviews.Add(productId, new Dictionary<string, ReviewDTO>());
            }
            Dictionary<string, ReviewDTO> temp = productReviews[productId];
            if (!temp.ContainsKey(user))
            {
                temp.Add(user, review);
            }
            else
            {
                temp[user] = review;
            }
        }
        private void AddToUserReviews(string user, int productId, ReviewDTO review)
        {
            if (!userReviews.ContainsKey(user))
            {
                userReviews.Add(user, new Dictionary<int, ReviewDTO>());
            }
            Dictionary<int, ReviewDTO> temp = userReviews[user];
            if (!temp.ContainsKey(productId))
            {
                temp.Add(productId, review);
            }
            else
            {
                temp[productId] = review;
            }
        }
    }
}