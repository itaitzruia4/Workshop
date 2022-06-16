﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class Review : DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string review { get; set; }
        public string reviewer { get; set; }
        public int productId { get; set; }

        public Review()
        {
        }

        public Review(string review, string reviewer, int productId)
        {
            this.review = review;
            this.reviewer = reviewer;
            this.productId = productId;
        }
    }
}
