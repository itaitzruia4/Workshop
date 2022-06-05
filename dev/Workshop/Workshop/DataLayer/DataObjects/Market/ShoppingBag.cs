using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Workshop.DataLayer.DataObjects.Market
{
    
    public class ShoppingBag : DALObject
    {
        public int StoreId { get; set; }
        public List<ShoppingBagProduct> Products { get; set; }

        public ShoppingBag(int storeId, List<ShoppingBagProduct> products)
        {
            StoreId = storeId;
            Products = products;
        }
    }
}
