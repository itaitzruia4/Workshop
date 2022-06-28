using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public class OrTerm: CompositeTerm
    {
        public OrTerm(Term firstTerm, Term secondTerm) : base(firstTerm, secondTerm)
        { }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            return firstTerm.IsEligible(shoppingBag) || secondTerm.IsEligible(shoppingBag);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag, int age)
        {
            return firstTerm.IsEligible(shoppingBag, age) || secondTerm.IsEligible(shoppingBag, age);
        }
    }
}
