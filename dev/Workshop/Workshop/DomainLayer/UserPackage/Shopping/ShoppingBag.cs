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
    public class ShoppingBag : IPersistentObject<ShoppingBagDAL>
    {
        private int storeId;
        private Dictionary<int, ShoppingBagProduct> products;
        private ShoppingBagDAL shoppingBagDAL;

        public ShoppingBag(int storeId)
        {
            this.storeId = storeId;
            products = new Dictionary<int,ShoppingBagProduct>();
            shoppingBagDAL = new ShoppingBagDAL(storeId, new List<ShoppingBagProductDAL>());
        }

        public ShoppingBag(ShoppingBagDAL shoppingBagDAL)
        {
            this.storeId = shoppingBagDAL.StoreId;
            products = new Dictionary<int, ShoppingBagProduct>(); //TODO: futher implement
            this.shoppingBagDAL = shoppingBagDAL;
        }

        public ShoppingBagDAL ToDAL()
        {
            return shoppingBagDAL;
        }

        public ShoppingBagProduct addToBag(ShoppingBagProduct product)
        {
            if (!products.ContainsKey(product.Id))
            {
                products.Add(product.Id, product);
                shoppingBagDAL.Products.Add(product.ToDAL());
                DataHandler.getDBHandler().update(shoppingBagDAL);
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
        internal void deleteProduct(int productId)
        {
            shoppingBagDAL.Products.Remove(products[productId].ToDAL());
            DataHandler.getDBHandler().remove(products[productId].ToDAL());
            products.Remove(productId);
            DataHandler.getDBHandler().update(shoppingBagDAL);
        }
        internal void changeQuantity(int productId,int newQuantity)
        {
            products[productId].Quantity = newQuantity;
        }
    }
}
