using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using DALTerm = Workshop.DataLayer.DataObjects.Market.Purchases.Term;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public abstract class Term : IPersistentObject<DALTerm>
    {
        public string json_discount { get; set; }
        public abstract bool IsEligible(ShoppingBagDTO shoppingBag);
        public abstract bool IsEligible(ShoppingBagDTO shoppingBag, int age);

        public DALTerm DALTerm { get; set; }
        public DALTerm ToDAL()
        {
            return this.DALTerm;
        }
    }
}
