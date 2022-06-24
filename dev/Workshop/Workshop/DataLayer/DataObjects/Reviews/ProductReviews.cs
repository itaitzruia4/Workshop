using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ProductReviews
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public List<UserToReviewDTO> userToReviewDTOs { get; set; }

        public ProductReviews()
        {
            this.Id = nextId;
            nextId++;
            userToReviewDTOs = new List<UserToReviewDTO>();
        }

        public ProductReviews(int productId, List<UserToReviewDTO> userToReviewDTOs)
        {
            this.ProductId = productId;
            this.userToReviewDTOs = userToReviewDTOs;
            this.Id = nextId;
            nextId++;
        }
    }
}
