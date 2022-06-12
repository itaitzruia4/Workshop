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
        public List<ShoppingBag> ShoppingBags { get; set; }

        public ShoppingCart(DomainShoppingCart shoppingCart)
        {
            this.ShoppingBags = shoppingCart.shoppingBags.Values.Select(x => new ShoppingBag(x)).ToList();
        }
    }
}
