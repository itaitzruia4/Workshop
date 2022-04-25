﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Workshop.DomainLayer.MarketPackage;

namespace Tests.UnitTests.DomainLayer.MarketPackage
{
    [TestClass]
    public class TestStore
    {
        private Store store;
        Product product;
        [TestInitialize]
        public void Setup()
        {
            store = new Store(1, "Store1");
            int id = 2;
            string name = "Product2";
            string description = "Product2Desc";
            double price = 102.5;
            int quantity = 5;
            product = store.AddProduct(id, name, description, price, quantity);
        }

        [TestMethod]
        public void AddProductSuccess()
        {
            int id = 1;
            string name = "Product1";
            string description = "Product1";
            double price = 9.99;
            int quantity = 10;
            Product p = store.AddProduct(id, name, description, price, quantity);
            Assert.IsNotNull(p);
            Assert.AreEqual(p.Name, name);
            Assert.AreEqual(p.Description, description);
            Assert.AreEqual(p.Price, price);
            Assert.AreEqual(p.Quantity, quantity);
            store.GetProduct(id);
        }

        [TestMethod]
        public void AddProductBadId()
        {
            int id = -1;
            string name = "Product1";
            string description = "Product1";
            double price = 9.99;
            int quantity = 10;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => store.AddProduct(id, name, description, price, quantity));
            Assert.ThrowsException<ArgumentException>(() => store.GetProduct(id));
        }

        [TestMethod]
        public void AddProductBadName()
        {
            int id = 1;
            string name = "";
            string description = "Product1";
            double price = 9.99;
            int quantity = 10;
            Assert.ThrowsException<ArgumentException>(() => store.AddProduct(id, name, description, price, quantity));
            Assert.ThrowsException<ArgumentException>(() => store.GetProduct(id));
        }

        [TestMethod]
        public void AddProductBadPrice()
        {
            int id = 1;
            string name = "Product1";
            string description = "Product1";
            double price = -0.9;
            int quantity = 10;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => store.AddProduct(id, name, description, price, quantity));
            Assert.ThrowsException<ArgumentException>(() => store.GetProduct(id));
        }

        [TestMethod]
        public void AddProductBadQuantity()
        {
            int id = 1;
            string name = "Product1";
            string description = "Product1";
            double price = 9.99;
            int quantity = -1;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => store.AddProduct(id, name, description, price, quantity));
            Assert.ThrowsException<ArgumentException>(() => store.GetProduct(id));
        }
        
        [TestMethod]
        public void RemoveProductSuccess()
        {
            int id = 1;
            string name = "Product1";
            string description = "Product1";
            double price = 9.99;
            int quantity = 10;
            Product p = store.AddProduct(id, name, description, price, quantity);
            store.GetProduct(id);
            store.RemoveProduct(id);
            Assert.ThrowsException<ArgumentException>(() => store.GetProduct(id));
        }

        [TestMethod]
        public void RemoveProductFail()
        {
            int id = 1;
            Assert.ThrowsException<Exception>(() => store.RemoveProduct(id));
        }

        [TestMethod]
        public void ChangeProductNameSuccess()
        {
            string name = "newName2";
            store.ChangeProductName(this.product.Id, name);
            Assert.AreEqual(name, this.product.Name);
        }

        [TestMethod]
        public void ChangeProductNameFail()
        {
            string name = "";
            string previousName = this.product.Name;
            Assert.ThrowsException<ArgumentException>(() => store.ChangeProductName(this.product.Id, name));
            Assert.AreEqual(previousName, this.product.Name);
        }

        [TestMethod]
        public void ChangeProductDescriptionSuccess()
        {
            string description = "newName2";
            store.ChangeProductDescription(this.product.Id, description);
            Assert.AreEqual(description, this.product.Description);
        }

        [TestMethod]
        public void ChangeProductDescriptionFail()
        {
            string description = "newDesc2";
            string previousDescription = this.product.Description;
            Assert.ThrowsException<Exception>(() => store.ChangeProductDescription(10, description));
            Assert.AreEqual(previousDescription, this.product.Description);
        }

        [TestMethod]
        public void ChangeProductPriceSuccess()
        {
            double price = 20.0;
            store.ChangeProductPrice(this.product.Id, price);
            Assert.AreEqual(price, this.product.Price);
        }

        [TestMethod]
        public void ChangeProductPriceFail()
        {
            double price = -3.0;
            double previousPrice = this.product.Price;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => store.ChangeProductPrice(this.product.Id, price));
            Assert.AreEqual(previousPrice, this.product.Price);
        }

        [TestMethod]
        public void ChangeProductQuantitySuccess()
        {
            int quantity = 20;
            store.ChangeProductQuantity(this.product.Id, quantity);
            Assert.AreEqual(quantity, this.product.Quantity);
        }

        [TestMethod]
        public void ChangeProductQuantityFail()
        {
            int quantity = -5;
            double previousQuantity = this.product.Quantity;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => store.ChangeProductQuantity(this.product.Id, quantity));
            Assert.AreEqual(previousQuantity, this.product.Quantity);
        }
    }
}
