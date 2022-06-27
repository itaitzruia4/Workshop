using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class PriceAction
    {
        public string json_price_action { get; set; }
        public abstract double CalculatePriceAction(ShoppingBagDTO shoppingBag);
    }
}
