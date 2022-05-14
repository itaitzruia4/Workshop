using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;
using DomainProductDTO = Workshop.DomainLayer.MarketPackage.ProductDTO;
using DomainShoppingBagProduct = Workshop.DomainLayer.MarketPackage.ShoppingBagProduct;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Product
    {
        public readonly int Id;
        public readonly string Name;
        public readonly double BasePrice;
        public readonly string Description;
        public readonly int Quantity;
        public readonly string Category;
        internal Product(int id, string name, string description, double basePrice, int quantity, string category)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            Description = description;
            Quantity = quantity;
            Category = category;
        }

        internal Product(DomainProduct domainProduct)
        {
            Id = domainProduct.Id;
            Name = domainProduct.Name;
            BasePrice = domainProduct.Price;
            Description = domainProduct.Description;
            Quantity = domainProduct.Quantity;
            Category = domainProduct.Category;
        }

        internal Product(DomainProductDTO domainProduct)
        {
            Id = domainProduct.Id;
            Name = domainProduct.Name;
            BasePrice = domainProduct.Price;
            Description = domainProduct.Description;
            Quantity = domainProduct.Quantity;
            Category = domainProduct.Category;
        }

        internal Product(DomainShoppingBagProduct domainProduct)
        {
            Id = domainProduct.Id;
            Name = domainProduct.Name;
            BasePrice = domainProduct.Price;
            Description = domainProduct.Description;
            Quantity = domainProduct.Quantity;
            Category = domainProduct.Category;
        }
    }
}
