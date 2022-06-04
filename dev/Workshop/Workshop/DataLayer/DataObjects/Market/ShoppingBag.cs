using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Workshop.DataLayer.DataObjects.Market
{
    
    public class ShoppingBag
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public List<ShoppingBagProduct> Products { get; set; }
    }
}
