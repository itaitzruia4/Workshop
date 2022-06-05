using System;

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
        public int StoreId { get; set; }
        public Product(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
            this.StoreId = StoreId;
        }

        public ProductDTO GetProductDTO()
        {
            return new ProductDTO(Id, Name, Description, Price, Quantity, Category, StoreId);
        }
        public ShoppingBagProduct GetShoppingBagProduct(int quantity)
        {
            return new ShoppingBagProduct(Id, Name, Description, Price, quantity, Category, StoreId);
        }
        public override bool Equals(Object product)
        {
            if (product == null || !(product is Product)) return false;
            return (this.Id == ((Product)product).Id) &&
                (this.Name == ((Product)product).Name) &&
                (this.Price == ((Product)product).Price) &&
                (this.Quantity == ((Product)product).Quantity) &&
                (this.Description == ((Product)product).Description) &&
                (this.Category == ((Product)product).Category) &&
                (this.StoreId == ((Product)product).StoreId);
        }
    }
}
