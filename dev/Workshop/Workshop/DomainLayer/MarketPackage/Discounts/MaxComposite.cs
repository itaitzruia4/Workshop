using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public class MaxComposite: PriceActionComposite
    {
        public MaxComposite(PriceAction firstAction, PriceAction secondAction) : base(firstAction, secondAction)
        { }

        public override double CalculatePriceAction(ShoppingBagDTO shoppingBag)
        {
            return Math.Max(firstAction.CalculatePriceAction(shoppingBag), secondAction.CalculatePriceAction(shoppingBag));
        }
    }
}
