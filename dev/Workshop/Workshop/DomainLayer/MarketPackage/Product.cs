using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public Product(int id, string name, string description, double price, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }

        public ProductDTO ToProductDTO()
        {
            return new ProductDTO(Id,Name,Description,Price,Quantity);
        }
        public ShoppingBagProduct ToGetShopingBagProduct(int quantity)
        {
            return new ShoppingBagProduct(Id,Name,Price,quantity);
        }
    }
}
