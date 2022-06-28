using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.MarketPackage.Terms
{
    public abstract class CompositeTerm: Term
    {
        protected Term firstTerm;
        protected Term secondTerm;

        public CompositeTerm(Term firstTerm, Term secondTerm)
        {
            this.firstTerm = firstTerm;
            this.secondTerm = secondTerm;
        }

        public override abstract bool IsEligible(ShoppingBagDTO shoppingBag);
        public override abstract bool IsEligible(ShoppingBagDTO shoppingBag, int age);
    }
}
