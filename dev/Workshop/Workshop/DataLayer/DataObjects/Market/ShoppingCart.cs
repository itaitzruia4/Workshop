using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ShoppingCart : DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<ShoppingBag> ShoppingBags { get; set; }

        public ShoppingCart()
        {
            ShoppingBags = new List<ShoppingBag>();
            this.Id = nextId;
            nextId++;
        }

        public ShoppingCart(List<ShoppingBag> shoppingBags)
        {
            ShoppingBags = shoppingBags;
            this.Id = nextId;
            nextId++;
        }
    }
}
