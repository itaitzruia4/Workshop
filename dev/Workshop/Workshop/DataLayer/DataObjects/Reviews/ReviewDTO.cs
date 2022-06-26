using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ReviewDTO: DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Review { get; set; }
        public string Reviewer { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }

        public ReviewDTO()
        {
            this.Id = nextId;
            nextId++;
        }

        public ReviewDTO(string review, string reviewer, int productId, int rating)
        {
            Review = review;
            Reviewer = reviewer;
            ProductId = productId;
            Rating = rating;
            this.Id = nextId;
            nextId++;
        }
    }
}
