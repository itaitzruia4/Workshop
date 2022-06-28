using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public class SimpleTerm: Term
    {
        private TermSimple simpleTerm;

        public delegate bool TermSimple(ShoppingBagDTO shoppingBag, int age);

        public SimpleTerm(TermSimple simpleTerm)
        {
            this.simpleTerm = simpleTerm;
        }
        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return simpleTerm(shoppingBag, 0);
        }
        public override bool IsEligible(ShoppingBagDTO shoppingBag, int age)
        {
            return simpleTerm(shoppingBag, age);
        }
    }
}
