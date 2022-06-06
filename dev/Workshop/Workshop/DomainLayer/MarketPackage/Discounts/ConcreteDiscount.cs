using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using DALObject = Workshop.DataLayer.DALObject;
using ConcreteDiscountDAL = Workshop.DataLayer.DataObjects.Market.Discounts.ConcreteDiscount;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class ConcreteDiscount: Discount
    {
        protected PriceAction priceAction;

        public ConcreteDiscount(PriceAction priceAction)
        {
            this.priceAction = priceAction;
        }

        public abstract DALObject ToDAL();

        public abstract double CalculateDiscountValue(ShoppingBagDTO shoppingBag);

        public abstract bool IsEligible(ShoppingBagDTO shoppingBag);
        
    }
}
