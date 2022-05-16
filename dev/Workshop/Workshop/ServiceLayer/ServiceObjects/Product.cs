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
        public int Id { get; set; }
        public string Name { get; set; }
        public double BasePrice { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
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
