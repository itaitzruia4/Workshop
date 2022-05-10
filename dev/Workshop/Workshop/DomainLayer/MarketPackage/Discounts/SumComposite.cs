using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class SumComposite: PriceActionComposite
    {
        public SumComposite(PriceAction firstAction, PriceAction secondAction): base(firstAction, secondAction)
        {}

        public override double CalculatePriceAction(ShoppingBagDTO shoppingBag)
        {
            return firstAction.CalculatePriceAction(shoppingBag) + secondAction.CalculatePriceAction(shoppingBag);
        }
    }
}
