using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public interface Term
    {
        bool IsEligible(ShoppingBagDTO shoppingBag);
        bool IsEligible(ShoppingBagDTO shoppingBag, int age);
    }
}
