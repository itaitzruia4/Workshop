using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class AndDiscount : CompositeDiscount
    {
        public AndDiscount(Discount firstDiscount, Discount secondDiscount): base(firstDiscount, secondDiscount)
        {

        }
        public override double CalculateDiscountValue(ShoppingBagDTO shoppingBag)
        {
            if (!IsEligible(shoppingBag))
                return 0;
            return firstDiscount.CalculateDiscountValue(shoppingBag) + secondDiscount.CalculateDiscountValue(shoppingBag);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return firstDiscount.IsEligible(shoppingBag) && secondDiscount.IsEligible(shoppingBag);
        }
    }
}
