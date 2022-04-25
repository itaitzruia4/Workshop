using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.MarketPackage
{
    public class ShoppingBagProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        private string Description { get; set; }

        public ShoppingBagProduct(int id, string name, string description, double price, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }
        public ProductDTO GetProductDTO()
        {
            return new ProductDTO(Id,Name,Description,Price,Quantity);
        }
    }
}
