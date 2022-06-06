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

namespace Workshop.DomainLayer.UserPackage
{
    public class Review : IPersistentObject
    {
        string review { get; set; }
        string reviewer { get; set; }
        int productId { get; set; }

        public Review(string user, int productId, string review)
        {
            ValidateReviewContent(review);
            this.reviewer = user;
            this.review = review;
            this.productId = productId;
        }

        public DALObject ToDAL()
        {
            return new ReviewDAL(review, reviewer, productId);
        }

        private void ValidateReviewContent(string review){
            if (String.IsNullOrWhiteSpace(review)){
                throw new ArgumentException("A review can not be empty.");
            }
        }
    }
}
