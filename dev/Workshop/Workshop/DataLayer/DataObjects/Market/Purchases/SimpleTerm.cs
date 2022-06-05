using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class SimpleTerm: Term
    {
        public int Id { get; set; }
        public TermSimple simpleTerm { get; set; }

        public delegate bool TermSimple(ShoppingBagDTO shoppingBag, int age);
    }
}
