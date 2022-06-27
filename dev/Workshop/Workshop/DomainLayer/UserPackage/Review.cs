using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using ReviewDAL = Workshop.DataLayer.DataObjects.Reviews.Review;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage
{
    public class Review : IPersistentObject<ReviewDAL>
    {
        string review { get; set; }
        string reviewer { get; set; }
        int productId { get; set; }
        ReviewDAL reviewDAL { get; set; }


        public Review(string reviewer, int productId, string review)
        {
            ValidateReviewContent(review);
            this.reviewer = reviewer;
            this.review = review;
            this.productId = productId;
            reviewDAL = new ReviewDAL(review, reviewer, productId);
            DataHandler.Instance.Value.save(reviewDAL);
        }

        public Review(ReviewDAL reviewDAL)
        {
            this.reviewer = reviewDAL.reviewer;
            this.review = reviewDAL.review;
            this.productId = reviewDAL.productId;
            this.reviewDAL = reviewDAL;
        }

        public ReviewDAL ToDAL()
        {
            return reviewDAL;
        }

        private void ValidateReviewContent(string review){
            if (String.IsNullOrWhiteSpace(review)){
                throw new ArgumentException("A review can not be empty.");
            }
        }
    }
}
