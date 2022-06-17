﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
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

        public ShoppingBagProduct AddToCart(ShoppingBagProduct product, int storeId)
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

        internal int GetQuantityInCart(int productId, int bagNum)
        {
            return shoppingBags[bagNum].GetQuantity(productId);
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
