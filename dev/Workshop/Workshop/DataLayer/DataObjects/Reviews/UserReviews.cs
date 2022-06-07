using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class UserReviews
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<ProductToReviewDTO> productToReviewDTOs { get; set; }

        public UserReviews()
        {
        }

        public UserReviews(string username, List<ProductToReviewDTO> productToReviewDTOs)
        {
            this.Username = username;
            this.productToReviewDTOs = productToReviewDTOs;
        }
    }
}
