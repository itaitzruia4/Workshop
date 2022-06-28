using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class PriceActionComposite: PriceAction
    {
        protected PriceAction firstAction;
        protected PriceAction secondAction;

        public PriceActionComposite(PriceAction firstAction, PriceAction secondAction)
        {
            this.firstAction = firstAction;
            this.secondAction = secondAction;
        }

        public abstract override double CalculatePriceAction(ShoppingBagDTO shoppingBag);
    }
}
