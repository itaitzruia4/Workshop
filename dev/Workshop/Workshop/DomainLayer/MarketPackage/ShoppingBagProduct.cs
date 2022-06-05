using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;
using DALObject = Workshop.DataLayer.DALObject;


namespace Workshop.DomainLayer.MarketPackage
{
    public class ShoppingBagProduct : IPersistentObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int StoreId { get; set; }

        public ShoppingBagProduct(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
            this.StoreId = StoreId;
        }

        public DALObject ToDAL()
        {
            return new ShoppingBagProductDAL(Name, Price, Quantity, Description, Category);
        }

        public ProductDTO GetProductDTO()
        {
            return new ProductDTO(Id, Name, Description, Price, Quantity, Category, StoreId);
        }
    }
}
