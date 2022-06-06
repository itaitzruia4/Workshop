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

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingCart : IPersistentObject
    {
        private Dictionary<int,ShoppingBag> shoppingBags { get; set; }

        public ShoppingCart()
        {
            shoppingBags = new Dictionary<int,ShoppingBag>();
        }

        public DALObject ToDAL()
        {
            List<ShoppingBagDAL> shoppingBagsDAL = new List<ShoppingBagDAL>();
            foreach(KeyValuePair<int, ShoppingBag> entry in shoppingBags)
            {
                shoppingBagsDAL.Add((ShoppingBagDAL)entry.Value.ToDAL());
            }

            return new ShoppingCartDAL(shoppingBagsDAL);
        }

        public ShoppingBagProduct addToCart(ShoppingBagProduct product, int storeId)
        {
            if(!checkIfStoreHasBag(storeId))
            {
                shoppingBags.Add(storeId,new ShoppingBag(storeId));
            }
            return shoppingBags[storeId].addToBag(product);
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
        }

        internal void deleteProduct(int productId, int bagNum)
        {
            shoppingBags[bagNum].deleteProduct(productId);
        }
        internal void changeQuantity(int productId,int newQuantity, int bagNum)
        {
            shoppingBags[bagNum].changeQuantity(productId,newQuantity);
        }
    }
}
