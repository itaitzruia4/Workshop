using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class UserReviews
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Username { get; set; }
        public List<ProductToReviewDTO> productToReviewDTOs { get; set; }

        public UserReviews()
        {
            this.Id = nextId;
            nextId++;
            productToReviewDTOs = new List<ProductToReviewDTO>();
        }

        public UserReviews(string username, List<ProductToReviewDTO> productToReviewDTOs)
        {
            this.Username = username;
            this.productToReviewDTOs = productToReviewDTOs;
            this.Id = nextId;
            nextId++;
        }
    }
}
