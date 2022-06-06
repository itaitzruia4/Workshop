﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Review { get; set; }
        public string Reviewer { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }

        public ReviewDTO()
        {
        }

        public ReviewDTO(int id, string review, string reviewer, int productId, int rating)
        {
            Id = id;
            Review = review;
            Reviewer = reviewer;
            ProductId = productId;
            Rating = rating;
        }
    }
}
