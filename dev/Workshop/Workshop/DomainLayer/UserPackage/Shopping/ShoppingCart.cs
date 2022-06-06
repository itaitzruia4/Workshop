using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using ShoppingCartDAL = Workshop.DataLayer.DataObjects.Market.ShoppingCart;
using DALObject = Workshop.DataLayer.DALObject;
using ShoppingBagDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBag;
using DataHandler = Workshop.DataLayer.DataHandler;
using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingCart : IPersistentObject<ShoppingCartDAL>
    {
        private Dictionary<int,ShoppingBag> shoppingBags { get; set; }
        ShoppingCartDAL shoppingCartDAL;

        public ShoppingCart()
        {
            this.shoppingBags = new Dictionary<int,ShoppingBag>();
            shoppingCartDAL = new ShoppingCartDAL(new List<ShoppingBagDAL>());
            DataHandler.getDBHandler().save(shoppingCartDAL);
        }

        public ShoppingCart(ShoppingCartDAL shoppingCartDAL)
        {
            shoppingBags = new Dictionary<int, ShoppingBag>();
            foreach(ShoppingBagDAL shoppingBagDAL in shoppingCartDAL.ShoppingBags)
            {
                shoppingBags.Add(shoppingBagDAL.StoreId, new ShoppingBag(shoppingBagDAL));
            }
            this.shoppingCartDAL = shoppingCartDAL;
        }

        public ShoppingCartDAL ToDAL()
        {
            return shoppingCartDAL;
        }

        public ShoppingBagProduct addToCart(ShoppingBagProduct product, int storeId)
        {
            if(!checkIfStoreHasBag(storeId))
            {
                shoppingBags.Add(storeId,new ShoppingBag(storeId));
                shoppingCartDAL.ShoppingBags.Add(new ShoppingBagDAL(storeId, new List<ShoppingBagProductDAL>()));
            }

            ShoppingBagProduct productRet = shoppingBags[storeId].addToBag(product);
            DataHandler.getDBHandler().update(shoppingCartDAL);
            return productRet;
        }
        public ShoppingCartDTO getShoppingCartDTO()
        {
            Dictionary<int,ShoppingBagDTO> shoppingBagsDTOs = new Dictionary<int, ShoppingBagDTO>();
            foreach (int key in shoppingBags.Keys)
            {
                shoppingBagsDTOs.Add(key,shoppingBags[key].GetShoppingBagDTO());
            }
            return new ShoppingCartDTO(shoppingBagsDTOs);
        }
        public int checkIfHasBag(int ProductId)
        {
            foreach (int key in shoppingBags.Keys)
            {
                if(shoppingBags[key].HasProduct(ProductId))
                {
                    return key;
                }
            }
            return -1;
        }

        public bool checkIfStoreHasBag(int StoreId)
        {
            return shoppingBags.ContainsKey(StoreId);
        }

        internal void Clear()
        {
            shoppingBags.Clear();
            foreach(ShoppingBagDAL sbDAL in shoppingCartDAL.ShoppingBags)
            {
                foreach(ShoppingBagProductDAL sbpDAL in sbDAL.Products)
                {
                    DataHandler.getDBHandler().remove(sbpDAL);
                }
                DataHandler.getDBHandler().remove(sbDAL);
            }
            shoppingCartDAL.ShoppingBags = new List<ShoppingBagDAL>();
        }

        internal void deleteProduct(int productId, int bagNum)
        {
            shoppingBags[bagNum].deleteProduct(productId);
        }
        internal void changeQuantity(int productId,int newQuantity, int bagNum)
        {
            shoppingBags[bagNum].changeQuantity(productId, newQuantity);
        }
    }
}
