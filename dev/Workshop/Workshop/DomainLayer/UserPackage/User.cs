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

        public ShoppingBagProduct AddToCart(ShoppingBagProduct product, int storeId)
        {
            return this.shoppingCart.AddToCart(product,storeId);
        }
        internal ShoppingCartDTO ViewShoppingCart()
        {
            return shoppingCart.getShoppingCartDTO();
        }

        internal int GetStoreOfProduct(int productId)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if (bagNum == -1)
            {
                throw new ArgumentException($"Product {productId} does not exist in the cart of this user");
            }
            return bagNum;
        }

        internal void deleteFromCart(int productId)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if (bagNum != -1)
            {
                shoppingCart.deleteProduct(productId, bagNum);
            }
            else throw new ArgumentException("Product doesnt exist in cart");
        }

        internal int GetQuantityInCart(int productId)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if (bagNum != -1)
            {
                return shoppingCart.GetQuantityInCart(productId, bagNum);
            }
            else throw new ArgumentException("Product doesnt exist in cart");
        }

        internal void changeQuantityInCart(int productId, int newQuantity)
        {
            int bagNum = shoppingCart.checkIfHasBag(productId);
            if(bagNum != -1)
            {
                shoppingCart.changeQuantity(productId, newQuantity,bagNum);
            }
            else throw new ArgumentException("Product doesnt exist in cart");
        }

        internal void ClearCart()
        {
            shoppingCart.Clear();
        }
    }
}
