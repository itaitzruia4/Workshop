using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;


namespace Workshop.DomainLayer.MarketPackage
{
    public class ShoppingBagProduct : IPersistentObject<ShoppingBagProductDAL>
    {
        public int Id { get; set; }
        public string Name { get => Name; set => setName(Name); }
        public double Price { get => Price; set => setPrice(Price); }
        public int Quantity { get => Quantity; set => setQuantity(Quantity); }
        public string Description { get => Name; set => setDescription(Description); }
        public string Category { get => Category; set => setCategory(Category);}
        public int StoreId { get; set; }

        private ShoppingBagProductDAL shoppingBagProductDAL;

        public ShoppingBagProduct(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
            this.StoreId = StoreId;
            shoppingBagProductDAL  = new ShoppingBagProductDAL(Name, Price, Quantity, Description, Category);
        }

        private void setName(string Name)
        {
            this.Name = Name;
            shoppingBagProductDAL.Name = Name;
            DataHandler.getDBHandler().update(shoppingBagProductDAL);
        }

        private void setPrice(double Price)
        {
            this.Price = Price;
            shoppingBagProductDAL.Price = Price;
            DataHandler.getDBHandler().update(shoppingBagProductDAL);
        }

        private void setQuantity(int Quantity)
        {
            this.Quantity = Quantity;
            shoppingBagProductDAL.Quantity = Quantity;
            DataHandler.getDBHandler().update(shoppingBagProductDAL);
        }

        private void setDescription(string Description)
        {
            this.Description = Description;
            shoppingBagProductDAL.Description = Description;
            DataHandler.getDBHandler().update(shoppingBagProductDAL);
        }

        private void setCategory(string Category)
        {
            this.Category = Category;
            shoppingBagProductDAL.Category = Category;
            DataHandler.getDBHandler().update(shoppingBagProductDAL);
        }

        public ShoppingBagProductDAL ToDAL()
        {
            return shoppingBagProductDAL;
        }

        public ProductDTO GetProductDTO()
        {
            return new ProductDTO(Id, Name, Description, Price, Quantity, Category, StoreId);
        }
    }
}
