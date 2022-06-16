using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ReviewHandler: DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<ProductReviews> productReviews { get; set; }
        public List<UserReviews> userReviews { get; set; }

        public ReviewHandler()
        {
        }

        public ReviewHandler(List<ProductReviews> productReviews, List<UserReviews> userReviews)
        {
            this.productReviews = productReviews;
            this.userReviews = userReviews;
        }
    }
}
