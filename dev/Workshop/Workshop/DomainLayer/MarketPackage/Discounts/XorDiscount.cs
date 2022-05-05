using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class XorDiscount: CompositeDiscount
    {
        private DiscountTerm DiscountTerm;
        public XorDiscount(Discount firstDiscount, Discount secondDiscount, DiscountTerm discountTerm) : base(firstDiscount, secondDiscount)
        {
            this.DiscountTerm = discountTerm;
        }
        public override double CalculateDiscountValue(ShoppingBagDTO shoppingBag)
        {
            if (!IsEligible(shoppingBag))
                return 0;
            if (firstDiscount.IsEligible(shoppingBag) && !secondDiscount.IsEligible(shoppingBag))
                return firstDiscount.CalculateDiscountValue(shoppingBag);
            if (!firstDiscount.IsEligible(shoppingBag) && secondDiscount.IsEligible(shoppingBag))
                return secondDiscount.CalculateDiscountValue(shoppingBag);

            // If both discounts holds, the discount term determines if to use the first discount or the second
            if (DiscountTerm.IsEligible(shoppingBag))
                return firstDiscount.CalculateDiscountValue(shoppingBag);

            return secondDiscount.CalculateDiscountValue(shoppingBag);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return firstDiscount.IsEligible(shoppingBag) || secondDiscount.IsEligible(shoppingBag);
        }
    }
}
