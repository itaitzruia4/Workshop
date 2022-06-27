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
using ReviewHandlerDAL = Workshop.DataLayer.DataObjects.Reviews.ReviewHandler;
using ProductReviewesDAL = Workshop.DataLayer.DataObjects.Reviews.ProductReviews;
using UserReviewesDAL = Workshop.DataLayer.DataObjects.Reviews.UserReviews;
using DataHandler = Workshop.DataLayer.DataHandler;
using UserToReviewDTO =  Workshop.DataLayer.DataObjects.Reviews.UserToReviewDTO;
using ProductToReviewDTO = Workshop.DataLayer.DataObjects.Reviews.ProductToReviewDTO;
using Workshop.DomainLayer.UserPackage;

namespace Workshop.DomainLayer.Reviews
{
    public class ReviewHandler : IReviewHandler
    {
        ConcurrentDictionary<int, ConcurrentDictionary<string, ReviewDTO>> productReviews;
        ConcurrentDictionary<string, ConcurrentDictionary<int, ReviewDTO>> userReviews;
        public ReviewHandlerDAL reviewHandlerDAL { get; set; }

        public ReviewHandler()
        {
            productReviews = new ConcurrentDictionary<int, ConcurrentDictionary<string, ReviewDTO>>();
            userReviews = new ConcurrentDictionary<string, ConcurrentDictionary<int, ReviewDTO>>();
            reviewHandlerDAL = new ReviewHandlerDAL(new List<ProductReviewesDAL>(), new List<UserReviewesDAL>());
            DataHandler.getDBHandler().save(reviewHandlerDAL);
        }

        public ReviewHandler(ReviewHandlerDAL reviewHandlerDAL)
        {
            productReviews = new ConcurrentDictionary<int, ConcurrentDictionary<string, ReviewDTO>>();
            userReviews = new ConcurrentDictionary<string, ConcurrentDictionary<int, ReviewDTO>>();
            foreach(ProductReviewesDAL prDAL in reviewHandlerDAL.productReviews)
            {
                ConcurrentDictionary<string, ReviewDTO> prDict = new ConcurrentDictionary<string, ReviewDTO>();
                foreach(UserToReviewDTO urtDTO in prDAL.userToReviewDTOs)
                {
                    prDict.TryAdd(urtDTO.Username, new ReviewDTO(urtDTO.Review));
                }
                productReviews.TryAdd(prDAL.ProductId, prDict);
            }
            foreach (UserReviewesDAL urDAL in reviewHandlerDAL.userReviews)
            {
                ConcurrentDictionary<int, ReviewDTO> prDict = new ConcurrentDictionary<int, ReviewDTO>();
                foreach (ProductToReviewDTO prtDTO in urDAL.productToReviewDTOs)
                {
                    prDict.TryAdd(prtDTO.ProductId, new ReviewDTO(prtDTO.Review));
                }
                userReviews.TryAdd(urDAL.Username, prDict);
            }
            this.reviewHandlerDAL = reviewHandlerDAL;
        }

        public ReviewHandlerDAL ToDAL()
        {
            return reviewHandlerDAL;
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

            ProductReviewesDAL prDAL = reviewHandlerDAL.productReviews.Find(x => x.ProductId == productId);
            if (prDAL == null)
            {
                prDAL = new ProductReviewesDAL(productId, new List<UserToReviewDTO>());
                reviewHandlerDAL.productReviews.Add(prDAL);
                UserToReviewDTO urtDTO = new UserToReviewDTO(user, review.ToDAL());
                prDAL.userToReviewDTOs.Add(urtDTO);
            }
            else
            {
                UserToReviewDTO urtDTO = prDAL.userToReviewDTOs.Find(t => t.Username == user);
                if (urtDTO == null)
                {
                    urtDTO = new UserToReviewDTO(user, review.ToDAL());
                    prDAL.userToReviewDTOs.Add(urtDTO);
                }
                else
                {
                    urtDTO.Review = review.ToDAL();
                }
                
            }
            DataHandler.getDBHandler().update(reviewHandlerDAL);//TODO figure it out with Nirdan
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

            UserReviewesDAL prDAL = reviewHandlerDAL.userReviews.Find(x => x.Username == user);
            if (prDAL == null)
            {
                prDAL = new UserReviewesDAL(user, new List<ProductToReviewDTO>());
                reviewHandlerDAL.userReviews.Add(prDAL);
                ProductToReviewDTO ptrDTO = new ProductToReviewDTO(productId, review.ToDAL());
                prDAL.productToReviewDTOs.Add(ptrDTO);
            }
            else
            {
                ProductToReviewDTO ptrDTO = prDAL.productToReviewDTOs.Find(t => t.Id == productId);
                if (ptrDTO == null)
                {
                    ptrDTO = new ProductToReviewDTO(productId, review.ToDAL());
                    prDAL.productToReviewDTOs.Add(ptrDTO);
                }
                else
                {
                    ptrDTO.Review = review.ToDAL();
                }

            }
            DataHandler.getDBHandler().update(reviewHandlerDAL);//TODO figure it out with Nirdan
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