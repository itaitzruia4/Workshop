using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ReviewHandler
    {
        public int Id { get; set; }
        Dictionary<int, Dictionary<string, ReviewDTO>> productReviews;
        Dictionary<string, Dictionary<int, ReviewDTO>> userReviews;
    }
}
