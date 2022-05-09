using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class DiscountXorTerm: DiscountCompositeTerm
    {
        public DiscountXorTerm(DiscountTerm firstTerm, DiscountTerm secondTerm) : base(firstTerm, secondTerm)
        { }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            bool firstResult = firstTerm.IsEligible(shoppingBag);
            bool secondResult = firstTerm.IsEligible(shoppingBag);
            return (firstResult && !secondResult) || (!firstResult && secondResult);
        }
    }
}
