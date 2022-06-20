using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;


namespace Workshop.DomainLayer.MarketPackage
{
    public class ShoppingBagProduct : IPersistentObject<ShoppingBagProductDAL>
    {
        public int Id { get; set; }

        private string _name;
        public string Name { get => _name; set {
                this._name = value;
                shoppingBagProductDAL.Name = value;
                DataHandler.getDBHandler().update(shoppingBagProductDAL);
            } }

        private double _price;
        public double Price { get => _price; set {
                this._price = value;
                shoppingBagProductDAL.Price = value;
                DataHandler.getDBHandler().update(shoppingBagProductDAL);
            } }

        private int _quantity;
        public int Quantity { get => _quantity; set {
                this._quantity = value;
                shoppingBagProductDAL.Quantity = value;
                DataHandler.getDBHandler().update(shoppingBagProductDAL);
            } }

        private string _description;
        public string Description { get => _description; set {
                this._description = value;
                shoppingBagProductDAL.Description = value;
                DataHandler.getDBHandler().update(shoppingBagProductDAL);
            } }

        private string _category;
        public string Category { get => _category; set {
                this._category = value;
                shoppingBagProductDAL.Category = value;
                DataHandler.getDBHandler().update(shoppingBagProductDAL);
            } }
        public int StoreId { get; set; }

        private ShoppingBagProductDAL shoppingBagProductDAL;

        public ShoppingBagProduct(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this._name = name;
            this._description = description;
            this._price = price;
            this._quantity = quantity;
            this._category = category;
            this.StoreId = StoreId;
            shoppingBagProductDAL  = new ShoppingBagProductDAL(Name, Price, Quantity, Description, Category);
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
