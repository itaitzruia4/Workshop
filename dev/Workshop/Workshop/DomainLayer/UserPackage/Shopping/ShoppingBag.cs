using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage
{
    class ShoppingBag
    {
        private string storeId;
        private Dictionary<int, List<ShoppingBagProduct>> products;

        public ShoppingBag(int storeId)
        {
            this.storeId = store;
            products = new Dictionary<int,List<ShoppingBagProduct>>();
        }

        public void addToBag(ShopingBagProduct product)
        {
            if(!products.ContainsKey(product.getId()))
            {
                products.add(product.getId(),product);
            }
            shoppingBags[product.getId()].Quantity +=product.Quantity;
        }
        internal ShoppingBagDTO GetShoppingBagDTO()
        {
            Dictionary<int,List<Product>> shoppingBagsDTOs = new Dictionary<int, List<Product>>();
            foreach (int key in products.Keys)
            {
                shoppingBagsDTOs.Add(key,products[key].getProductDTO());
            }
            return new ShoppingBagDTO(storeId,shoppingBagsDTOs);
        }
        public int HasProduct(int ProductId)
        {
            return products.ContainsKey(ProductId);
        }
        internal void deleteProduct(int productId)
        {
            products.Remove(productId);
        }
        internal void changeQuantity(int productId,int newQuantity)
        {
            products[productId].Quantity = newQuantity;
        }
    }
}
