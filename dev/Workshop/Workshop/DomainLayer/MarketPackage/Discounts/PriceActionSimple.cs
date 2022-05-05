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

        public PriceActionSimple(double percentage) 
        {
            this.percentage = percentage; 
        }

        public double CalculatePriceAction(ShoppingBagDTO shoppingBag)
        {
            return percentage;
        }
    }
}
