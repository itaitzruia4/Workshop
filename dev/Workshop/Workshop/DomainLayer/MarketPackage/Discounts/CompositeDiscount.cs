using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class CompositeDiscount: Discount
    {
        protected Discount firstDiscount;
        protected Discount secondDiscount;

        public CompositeDiscount(Discount firstDiscount, Discount secondDiscount)
        {
            this.firstDiscount = firstDiscount;
            this.secondDiscount = secondDiscount;
        }

        public abstract double CalculateDiscountValue(ShoppingBagDTO shoppingBag);

        public abstract bool IsEligible(ShoppingBagDTO shoppingBag);
    }
}
