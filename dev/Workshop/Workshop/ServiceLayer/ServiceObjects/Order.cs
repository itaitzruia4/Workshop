using System;
using System.Collections.Generic;
using System.Linq;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerName { get; set; }
        public SupplyAddress Address { get; set; }
        public string StoreName { get; set; }
        public List<Product> Products { get; set; }
        public string Date { get; set; }
        public double Price { get; set; }

        public Order(DomainLayer.Orders.OrderDTO dor)
        {
            Id = dor.id;
            BuyerName = dor.clientName;
            Address = dor.address;
            StoreName = dor.storeName;
            Products = dor.items.Select(pdto => new Product(pdto)).ToList();
            Date = dor.date.ToShortDateString();
            Price = dor.price;
        }
    }
}
