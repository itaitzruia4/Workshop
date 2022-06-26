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
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<ProductReviews> productReviews { get; set; }
        public List<UserReviews> userReviews { get; set; }

        public ReviewHandler()
        {
            this.Id = nextId;
            nextId++;
            productReviews = new List<ProductReviews>();
            userReviews = new List<UserReviews>();
        }

        public ReviewHandler(List<ProductReviews> productReviews, List<UserReviews> userReviews)
        {
            this.productReviews = productReviews;
            this.userReviews = userReviews;
            this.Id = nextId;
            nextId++;
        }
    }
}
