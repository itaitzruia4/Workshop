using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class DiscountPolicy : DALObject
    {
        private List<Discount> products_discounts;
        private List<Discount> category_discounts;
        private Discount store_discount;
        private Store store;

        public DiscountPolicy(List<Discount> products_discounts, List<Discount> category_discounts, Discount store_discount, Store store)
        {
            this.products_discounts = products_discounts;
            this.category_discounts = category_discounts;
            this.store_discount = store_discount;
            this.store = store;
        }
    }
}
