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
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ReviewDTO Review { get; set; }

        public ProductToReviewDTO()
        {
            this.Id = nextId;
            nextId++;
        }

        public ProductToReviewDTO(int productId, ReviewDTO review)
        {
            this.ProductId = productId;
            this.Review = review;
            this.Id = nextId;
            nextId++;
        }
    }
}
