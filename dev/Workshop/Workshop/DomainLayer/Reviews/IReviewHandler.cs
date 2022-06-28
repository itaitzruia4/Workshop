using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;
using ReviewHandlerDAL = Workshop.DataLayer.DataObjects.Reviews.ReviewHandler;

namespace Workshop.DomainLayer.Reviews
{
    public interface IReviewHandler: IPersistentObject<ReviewHandlerDAL>
    { 
        ReviewDTO AddReview(string user, int productId, string review, int rating);
        double GetProductRating(int productId);
    }
}
