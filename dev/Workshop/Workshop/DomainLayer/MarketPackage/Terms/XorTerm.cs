using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public class XorTerm: CompositeTerm
    {
        public XorTerm(Term firstTerm, Term secondTerm) : base(firstTerm, secondTerm)
        { }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            bool firstResult = firstTerm.IsEligible(shoppingBag);
            bool secondResult = firstTerm.IsEligible(shoppingBag);
            return (firstResult && !secondResult) || (!firstResult && secondResult);
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag, int age)
        {
            bool firstResult = firstTerm.IsEligible(shoppingBag, age);
            bool secondResult = firstTerm.IsEligible(shoppingBag, age);
            return (firstResult && !secondResult) || (!firstResult && secondResult);
        }
    }
}
