using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using ReviewDTODAL = Workshop.DataLayer.DataObjects.Reviews.ReviewDTO;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.Reviews
{
    public class ReviewDTO: IPersistentObject<ReviewDTODAL>
    {
        public string Review { get; set; }
        public string Reviewer { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public ReviewDTODAL reviewDTODAL { get; set; }

        public ReviewDTO(string user, int productId, string review, int rating)
        {
            ValidateRating(rating);
            ValidateReview(review);
            this.Reviewer = user;
            this.Review = review;
            this.ProductId = productId;
            this.Rating = rating;
            this.reviewDTODAL = new ReviewDTODAL(review, user, productId, rating);
            DataHandler.getDBHandler().save(reviewDTODAL);
        }

        public ReviewDTO(ReviewDTODAL reviewDTODAL)
        {
            this.Reviewer = reviewDTODAL.Reviewer;
            this.Review = reviewDTODAL.Review;
            this.ProductId = reviewDTODAL.ProductId;
            this.Rating = reviewDTODAL.Rating;
            this.reviewDTODAL = reviewDTODAL;
        }
        public ReviewDTODAL ToDAL()
        {
            return this.reviewDTODAL;
        }

        private void ValidateReview(string review)
        {
            if (review == null || String.IsNullOrWhiteSpace(review))
                throw new ArgumentException("A review can not be empty.");
        }

        private void ValidateRating(int rating)
        {
            if (rating < 1 || rating > 10)
            {
                throw new ArgumentOutOfRangeException("Rating for a review has to be between 1 and 10");
            }
        }

    }
}