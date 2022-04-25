using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingCart
    {
        private Dictionary<int,ShoppingBag> shoppingBags { get; set; }

        public ShoppingCart()
        {
            shoppingBags = new Dictionary<int,ShoppingBag>();
        }

        public void addToCart(ShoppingBagProduct product, int storeId)
        {
            if(!checkIfHasBag(storeId))
            {
                shoppingBags.add(storeId,new ShoppingBag(storeId));
            }
            shoppingBags[storeId].addToBag(product);
        }
        public ShoppingCartDTO getShoppingCartDTO()
        {
            Dictionary<int,ShoppingBagDTO> shoppingBagsDTOs = new Dictionary<int, ShoppingBagDTO>();
            foreach (int key in shoppingBags.Keys)
            {
                shoppingBagsDTOs.Add(key,shoppingBags[key].getShopingBagDTO());
            }
            return new ShoppingCartDTO(shoppingBagsDTOs);
        }
        public int HasProduct(int ProductId)
        {
            foreach (int key in shoppingBags.Keys)
            {
                if(shoppingBag[key].HasProduct(ProductId))
                {
                    return key;
                }
            }
            return -1;
        }
        internal void deleteProduct(int productId, int bagNum)
        {
            shoppingBags[bagNum].deleteProduct(productId);
        }
        internal void changeQuantity(int productId,int newQuantity, int bagNum)
        {
            shoppingBags[bagNum].deleteProduct(productId,newQuantity);
        }
    }
}
