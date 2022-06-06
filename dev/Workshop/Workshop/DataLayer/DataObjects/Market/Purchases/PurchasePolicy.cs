using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class PurchasePolicy : DALObject
    {
        public int Id { get; set; }
        public Term firstTerm { get; set; }
        public Term secondTerm { get; set; }
        public PurchasePolicy()
        {
        }
    }
}
