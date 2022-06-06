using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ProductToReviewDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ReviewDTO Review { get; set; }
    }
}
