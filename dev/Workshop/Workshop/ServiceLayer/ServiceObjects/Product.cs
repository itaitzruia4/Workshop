using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Product
    {
        public readonly int Id;
        public readonly string Name;
        public readonly double BasePrice;
        public readonly string Description;
        public readonly int Quantity;

        internal Product(int id, string name, string description, double basePrice, int quantity)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            Description = description;
            Quantity = quantity;
        }

        internal Product(DomainProduct domainProduct)
        {
            Id = domainProduct.Id;
            Name = domainProduct.Name;
            BasePrice = domainProduct.Price;
            Description = domainProduct.Description;
            Quantity = domainProduct.Quantity;
        }
    }
}
