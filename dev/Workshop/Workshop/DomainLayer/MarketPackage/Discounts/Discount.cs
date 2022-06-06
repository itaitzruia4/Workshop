using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public interface Discount : IPersistentObject
    {
        double CalculateDiscountValue(ShoppingBagDTO shoppingBag);
        bool IsEligible(ShoppingBagDTO shoppingBag);
    }
}
