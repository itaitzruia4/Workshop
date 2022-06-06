﻿using System;
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

        public ProductToReviewDTO()
        {
        }

        public ProductToReviewDTO(int id, int productId, ReviewDTO review)
        {
            this.Id = id;
            this.ProductId = productId;
            this.Review = review;
        }
    }
}
