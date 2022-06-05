using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market.Purchases;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class DiscountPolicy : DALObject
    {
        public int Id { get; set; }
        public List<ProductTerms> products_terms { get; set; }
        public List<CategoryTerms> category_terms { get; set; }
        public Term user_terms { get; set; }
        public Term store_terms { get; set; }
        public DiscountPolicy()
        {
            
        }
    }
}
