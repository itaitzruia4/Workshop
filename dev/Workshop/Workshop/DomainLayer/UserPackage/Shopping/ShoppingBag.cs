using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingBag
    {
        private int storeId;
        private Dictionary<int, ShoppingBagProduct> products;

        public ShoppingBag(int storeId)
        {
            this.storeId = storeId;
            products = new Dictionary<int,ShoppingBagProduct>();
        }

        public ShoppingBagProduct addToBag(ShoppingBagProduct product)
        {
            if(!products.ContainsKey(product.Id))
            {
                products.Add(product.Id,product);
            }
            products[product.Id].Quantity +=product.Quantity;
            return product;
        }
        internal ShoppingBagDTO GetShoppingBagDTO()
        {
            List<ProductDTO> productsDTOs = new List<ProductDTO>();
            foreach (int key in products.Keys)
            {
                productsDTOs.Add(products[key].GetProductDTO());
            }
            return new ShoppingBagDTO(storeId, productsDTOs);
        }
        public bool HasProduct(int ProductId)
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
