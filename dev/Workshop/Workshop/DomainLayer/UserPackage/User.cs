using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage
{
    public class User
    {
        private ShoppingCart shoppingCart;

        public User()
        {
            shoppingCart = new ShoppingCart();
        }

        public ProductDTO addToCart(ShoppingBagProduct product, int storeId)
        {
            return this.shoppingCart.addToCart(product,storeId);
        }
        internal ShoppingCartDTO viewShopingCart()
        {
            return shoppingCart.getShoppingCartDTO();
        }
        internal void deleteFromCart(int productId)
        {
            int bagNum = shoppingCart.HasProduct(productId);
            if(bagNum != -1)
            {
                shoppingCart.deleteProduct(productId,bagNum);
            }
        }
        internal void changeQuantityInCart(int productId, int newQuantity)
        {
            int bagNum = shoppingCart.HasProduct(productId);
            if(bagNum != -1)
            {
                shoppingCart.changeQuantity(productId, newQuantity,bagNum);
            }
        }
    }
}
