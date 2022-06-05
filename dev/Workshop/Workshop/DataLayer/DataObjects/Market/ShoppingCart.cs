using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ShoppingCart : DALObject
    {
        public List<ShoppingBag> ShoppingBags { get; set; }

        public ShoppingCart(List<ShoppingBag> shoppingBags)
        {
            ShoppingBags = shoppingBags;
        }
    }
}
