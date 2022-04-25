using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.Reviews
{
    public interface IReviewHandler
    {
        ReviewDTO AddReview(string user, int productId, string review);
    }
}
