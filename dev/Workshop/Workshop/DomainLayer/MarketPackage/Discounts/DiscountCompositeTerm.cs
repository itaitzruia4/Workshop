using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class DiscountCompositeTerm: DiscountTerm
    {
        protected DiscountTerm firstTerm;
        protected DiscountTerm secondTerm;

        public DiscountCompositeTerm(DiscountTerm firstTerm, DiscountTerm secondTerm)
        {
            this.firstTerm = firstTerm;
            this.secondTerm = secondTerm;
        }

        public abstract bool IsEligible(ShoppingBagDTO shoppingBag);
    }
}
