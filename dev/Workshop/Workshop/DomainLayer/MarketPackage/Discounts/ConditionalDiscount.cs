using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage.Terms;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class ConditionalDiscount: ConcreteDiscount
    {
        private Term discountTerm;
        public ConditionalDiscount(PriceAction priceAction, Term discountTerm): base(priceAction)
        {
            this.discountTerm = discountTerm;
        }

        public override double CalculateDiscountValue(ShoppingBagDTO shoppingBag)
        {
            if (!discountTerm.IsEligible(shoppingBag))
                return 0;
            return priceAction.CalculatePriceAction(shoppingBag);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return discountTerm.IsEligible(shoppingBag);
        }
    }
}
