using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public class ConditionedTerm : CompositeTerm
    {
        public ConditionedTerm(Term firstTerm, Term secondTerm) : base(firstTerm, secondTerm)
        { }

        public override bool IsEligible(ShoppingBagDTO shoppingBag)
        {
            if (firstTerm.IsEligible(shoppingBag))
                return secondTerm.IsEligible(shoppingBag);
            return false;
        }

        public override bool IsEligible(ShoppingBagDTO shoppingBag, int age)
        {
            if (firstTerm.IsEligible(shoppingBag, age))
                return secondTerm.IsEligible(shoppingBag, age);
            return false;
        }
    }
}
