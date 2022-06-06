using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    internal class CompositeDiscount
    {
        protected Discount firstDiscount;
        protected Discount secondDiscount;

        public CompositeDiscount(Discount firstDiscount, Discount secondDiscount)
        {
            this.firstDiscount = firstDiscount;
            this.secondDiscount = secondDiscount;
        }
    }
}
