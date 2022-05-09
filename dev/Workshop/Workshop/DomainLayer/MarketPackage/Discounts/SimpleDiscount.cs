using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class SimpleDiscount: ConcreteDiscount
    {
        public SimpleDiscount(PriceAction priceAction): base(priceAction)
        { }

        public override double CalculateDiscountValue(ShoppingBagDTO shoppingBag)
        {
            return priceAction.CalculatePriceAction(shoppingBag);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return priceAction.CalculatePriceAction(shoppingBag) > 0;
        }
    }
}
