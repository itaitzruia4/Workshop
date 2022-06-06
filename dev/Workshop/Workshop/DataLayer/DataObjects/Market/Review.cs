using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class Review : DALObject
    {
        public string review { get; set; }
        public string reviewer { get; set; }
        public int productId { get; set; }

        public Review(string review, string reviewer, int productId)
        {
            this.review = review;
            this.reviewer = reviewer;
            this.productId = productId;
        }
    }
}
