using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class PriceActionSimple: PriceAction
    {
        private double percentage;
        private Filter filter;

        // The filter recieves a Product and returns true if the discount is valid for the Product
        public delegate bool Filter(ProductDTO product);

        public PriceActionSimple(double percentage, Filter filter) 
        {
            this.percentage = percentage;
            this.filter = filter;
        }

        public double CalculatePriceAction(ShoppingBagDTO shoppingBag)
        {
            double discount = 0;
            foreach (ProductDTO item in shoppingBag.products)
            {
                if (filter(item))
                    discount += ((percentage / 100) * item.Price)*item.Quantity;
            }
            return discount;
        }
    }
}
