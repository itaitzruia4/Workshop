using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ProductToReviewDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ReviewDTO Review { get; set; }

        public ProductToReviewDTO()
        {
        }

        public ProductToReviewDTO(int productId, ReviewDTO review)
        {
            this.ProductId = productId;
            this.Review = review;
        }
    }
}
