using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public List<ShoppingBag> ShoppingBags { get; set; }
    }
}
