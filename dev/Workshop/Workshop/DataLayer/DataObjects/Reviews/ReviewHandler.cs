using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ReviewHandler
    {
        public int Id { get; set; }
        public List<ProductReviews> productReviews { get; set; }
        public List<UserReviews> userReviews { get; set; }

        public ReviewHandler()
        {
        }

        public ReviewHandler(int id, List<ProductReviews> productReviews, List<UserReviews> userReviews)
        {
            this.Id = id;
            this.productReviews = productReviews;
            this.userReviews = userReviews;
        }
    }
}
