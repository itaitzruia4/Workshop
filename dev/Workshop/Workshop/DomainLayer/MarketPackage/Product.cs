using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public Product(int id, string name, string description, double price, int quantity, string category)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
        }

        public ProductDTO GetProductDTO()
        {
            return new ProductDTO(Id,Name,Description,Price,Quantity,Category);
        }
        public ShoppingBagProduct GetShoppingBagProduct(int quantity)
        {
            return new ShoppingBagProduct(Id,Name,Description,Price,quantity,Category);
        }
        public override bool Equals(Object product)
        {
            if (product == null || !(product is Product)) return false;
            return (this.Id == ((Product)product).Id) &&
                (this.Name == ((Product)product).Name) &&
                (this.Price == ((Product)product).Price) &&
                (this.Quantity == ((Product)product).Quantity) &&
                (this.Description == ((Product)product).Description) &&
                (this.Category == ((Product)product).Category);
        }
    }
}
