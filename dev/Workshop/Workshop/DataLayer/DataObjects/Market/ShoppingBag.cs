using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workshop.DataLayer.DataObjects.Market
{
    
    public class ShoppingBag : DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int StoreId { get; set; }
        public List<ShoppingBagProduct> Products { get; set; }

        public ShoppingBag()
        {}

        public ShoppingBag(int storeId, List<ShoppingBagProduct> products)
        {
            StoreId = storeId;
            Products = products;
        }
    }
}
