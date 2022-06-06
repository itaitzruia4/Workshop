using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ProductReviews
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public List<UserToReviewDTO> userToReviewDTOs { get; set; }
    }
}
