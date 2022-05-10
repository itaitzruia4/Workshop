using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class DiscountOrTerm: DiscountCompositeTerm
    {
        public DiscountOrTerm(DiscountTerm firstTerm, DiscountTerm secondTerm) : base(firstTerm, secondTerm)
        { }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return firstTerm.IsEligible(shoppingBag) || secondTerm.IsEligible(shoppingBag);
        }
    }
}
