using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public interface PriceAction : IPersistentObject
    {
        double CalculatePriceAction(ShoppingBagDTO shoppingBag);
    }
}
