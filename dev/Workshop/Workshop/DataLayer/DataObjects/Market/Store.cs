﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market.Discounts;
<<<<<<< HEAD
=======
using Workshop.DataLayer.DataObjects.Market.Purchases;
>>>>>>> a435401cdcd8cb032971602a6846e124fbc1c81b

namespace Workshop.DataLayer.DataObjects.Market
{
    public class Store : DALObject
    {
        public int Id { get; set; }
        public bool Open { get; set; }
        public string StoreName { get; set; }
        public DiscountPolicy DiscountPolicy { get; set; }
        public PurchasePolicy PurchasePolicy { get; set; }
        public List<Product> Products { get; set; }

        public Store()
        { }
        public Store(int id, bool open, string storeName, DiscountPolicy discountPolicy, PurchasePolicy purchasePolicy, List<Product> products)
        {
            Id = id;
            Open = open;
            StoreName = storeName;
            DiscountPolicy = discountPolicy;
            PurchasePolicy = purchasePolicy;
            Products = products;
        }
    }
}
