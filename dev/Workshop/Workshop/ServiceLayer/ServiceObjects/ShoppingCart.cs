using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShoppingCart = Workshop.DomainLayer.UserPackage.Shopping.ShoppingCartDTO;
using DomainShoppingBag = Workshop.DomainLayer.UserPackage.Shopping.ShoppingBagDTO;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class ShoppingCart
    {
        public Dictionary<int, ShoppingBag> shoppingBags { get; set; }

        public ShoppingCart(Dictionary<int, ShoppingBag> shoppingBags)
        {
            this.shoppingBags = shoppingBags;
        }
        public ShoppingCart(DomainShoppingCart shoppingCart)
        {
            foreach (int key in shoppingCart.shoppingBags.Keys)
            {
                this.shoppingBags.Add(key,new ShoppingBag(shoppingCart.shoppingBags[key]));
            }
        }
    }
}
