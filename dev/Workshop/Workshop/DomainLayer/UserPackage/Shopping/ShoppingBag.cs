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

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingBag : IPersistentObject
    {
        private int storeId;
        private Dictionary<int, ShoppingBagProduct> products;

        public ShoppingBag(int storeId)
        {
            this.storeId = storeId;
            products = new Dictionary<int,ShoppingBagProduct>();
        }

        public DALObject ToDAL()
        {
            List<ShoppingBagProductDAL> productsDAL = new List<ShoppingBagProductDAL>();
            foreach (KeyValuePair<int, ShoppingBagProduct> entry in products)
            {
                productsDAL.Add((ShoppingBagProductDAL)entry.Value.ToDAL());
            }

            return new ShoppingBagDAL(-1, storeId, productsDAL);
        }

        public ShoppingBagProduct addToBag(ShoppingBagProduct product)
        {
            if(!products.ContainsKey(product.Id))
            {
                products.Add(product.Id,product);
            }
            else products[product.Id].Quantity +=product.Quantity;
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
