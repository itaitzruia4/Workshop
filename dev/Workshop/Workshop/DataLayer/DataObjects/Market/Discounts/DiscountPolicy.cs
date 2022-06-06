using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
 
=======
using Workshop.DataLayer.DataObjects.Market.Purchases;
>>>>>>> a435401cdcd8cb032971602a6846e124fbc1c81b

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class DiscountPolicy : DALObject
    {
<<<<<<< HEAD
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
=======
        public int Id { get; set; }
        public List<ProductTerms> products_terms { get; set; }
        public List<CategoryTerms> category_terms { get; set; }
        public Term user_terms { get; set; }
        public Term store_terms { get; set; }
        public DiscountPolicy()
        {
            
>>>>>>> a435401cdcd8cb032971602a6846e124fbc1c81b
        }
    }
}
