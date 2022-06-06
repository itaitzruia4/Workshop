using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market.Purchases;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class ProductTerms
    {
        [Key]
        public int ProductId { get; set; }
        public Term Terms { get; set; }
    }
}
