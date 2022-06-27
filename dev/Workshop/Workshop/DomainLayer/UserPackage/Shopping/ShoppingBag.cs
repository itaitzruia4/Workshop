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
        protected int storeId;
        protected Dictionary<int, ShoppingBagProduct> products;

        protected ShoppingBag(){}

        public ShoppingBag(int storeId)
        {
            this.storeId = storeId;
            products = new Dictionary<int,ShoppingBagProduct>();
        }

        public int StoreId { get { return storeId; } }

        public ShoppingBagProduct addToBag(ShoppingBagProduct product)
        {
            if (!products.ContainsKey(product.Id))
            {
                products.Add(product.Id, product);
            }
            else 
                changeQuantity(product.Id, products[product.Id].Quantity + product.Quantity);
            
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
        internal virtual void deleteProduct(int productId)
        {
            //DataHandler.getDBHandler().remove(products[productId].ToDAL());
            products.Remove(productId);
        }
        internal virtual void changeQuantity(int productId,int newQuantity)
        {
            products[productId].Quantity = newQuantity;
        }

        internal int GetQuantity(int productId)
        {
            return products[productId].Quantity;
        }
    }
}
