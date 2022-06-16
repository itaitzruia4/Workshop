using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using ShoppingBagDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBag;
using DALObject = Workshop.DataLayer.DALObject;
using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;
using DataHandler = Workshop.DataLayer.DataHandler;

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

        public virtual ShoppingBagProduct addToBag(ShoppingBagProduct product)
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
    }
}
