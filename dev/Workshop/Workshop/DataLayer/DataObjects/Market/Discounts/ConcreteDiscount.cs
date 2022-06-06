using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public abstract class ConcreteDiscount
    {
        protected PriceAction priceAction;

        protected ConcreteDiscount(PriceAction priceAction)
        {
            this.priceAction = priceAction;
        }
    }
}
