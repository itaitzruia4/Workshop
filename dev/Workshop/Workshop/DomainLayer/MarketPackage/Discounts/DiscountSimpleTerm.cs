using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class DiscountSimpleTerm: DiscountTerm
    {
        private SimpleTerm simpleTerm;

        public delegate bool SimpleTerm(ShoppingBagDTO shoppingBag);
        public DiscountSimpleTerm(SimpleTerm simpleTerm)
        {
            this.simpleTerm = simpleTerm;
        }
        public bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return simpleTerm(shoppingBag);
        }
    }
}
