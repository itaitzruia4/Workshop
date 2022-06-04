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
        public string Category { get; set; }
        public int StoreId { get; set; }

        public ProductDTO(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
            this.StoreId = StoreId;
        }
        public override bool Equals(Object product)
        {
            if (product == null || !(product is ProductDTO)) return false;
            return (this.Id == ((ProductDTO)product).Id) &&
                (this.Name == ((ProductDTO)product).Name) &&
                (this.Price == ((ProductDTO)product).Price) &&
                (this.Quantity == ((ProductDTO)product).Quantity) &&
                (this.Description == ((ProductDTO)product).Description) &&
                (this.Category == ((ProductDTO)product).Category) &&
                (this.StoreId == ((ProductDTO)product).StoreId);
        }
    }
}