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
        protected ShoppingCart shoppingCart;

        public User()
        {
            shoppingCart = new ShoppingCart();
        }

        public ShoppingBagProduct addToCart(ShoppingBagProduct product, int storeId)
        {
            return shoppingCart.addToCart(product,storeId);
        }
        internal ShoppingCartDTO viewShopingCart()
        {
            return shoppingCart.getShoppingCartDTO();
        }
        internal void deleteFromCart(int productId)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if (bagNum != -1)
            {
                shoppingCart.deleteProduct(productId, bagNum);
            }
            else throw new ArgumentException("product doesnt exsist in cart");
        }
        internal void changeQuantityInCart(int productId, int newQuantity)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if(bagNum != -1)
            {
                shoppingCart.changeQuantity(productId, newQuantity,bagNum);
            }
            else throw new ArgumentException("product doesnt exsist in cart");
        }

        internal void ClearCart()
        {
            shoppingCart.Clear();
        }
    }
}
