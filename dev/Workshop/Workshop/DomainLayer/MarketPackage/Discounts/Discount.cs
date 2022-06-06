using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer;
using Workshop.DomainLayer.UserPackage.Shopping;
using DALDiscount = Workshop.DataLayer.DataObjects.Market.Discounts.Discount;
using DALObject = Workshop.DataLayer.DALObject;

namespace Workshop.DomainLayer.MarketPackage.Discounts
{
    public abstract class Discount : IPersistentObject<DALDiscount>
    {
        public string json_discount { get; set; }
        public abstract double CalculateDiscountValue(ShoppingBagDTO shoppingBag);
        public abstract bool IsEligible(ShoppingBagDTO shoppingBag);
        public DALDiscount ToDAL()
        {
            return new DALDiscount(json_discount);
        }
    }
}
