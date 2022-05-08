
using System;

namespace Workshop.DomainLayer.MarketPackage
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ProductDTO(int id, string name, string description, double price, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }
        public bool EqualsFields(ProductDTO product)
        {
            return (this.Id == product.Id) && (this.Name == product.Name) && (this.Price == product.Price) && (this.Quantity == product.Quantity) && (this.Description == product.Description);
        }
    }
}