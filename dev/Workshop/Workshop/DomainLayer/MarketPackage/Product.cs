using System;
using ProductDAL = Workshop.DataLayer.DataObjects.Market.Product;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Product : IPersistentObject<ProductDAL>
    {
        public int Id { get; set; }

        private string _name;
        public string Name { get => _name; set {
                this._name = value;
                productDAL.Name = value;
                DataHandler.getDBHandler().update(productDAL);
            } }

        private double _price;
        public double Price { get => _price; set {
                this._price = value;
                productDAL.Price = value;
                DataHandler.getDBHandler().update(productDAL);
            } }

        private int _quantity;
        public int Quantity { get => _quantity; set {
                this._quantity = value;
                productDAL.Quantity = value;
                DataHandler.getDBHandler().update(productDAL);
            } }

        private string _description;
        public string Description { get => _description; set {
                this._description = value;
                productDAL.Description = value;
                DataHandler.getDBHandler().update(productDAL);
            } }

        private string _category;
        public string Category { get => _category; set {
                this._category = value;
                productDAL.Category = value;
                DataHandler.getDBHandler().update(productDAL);
            } }
        public int StoreId { get; set; }

        private ProductDAL productDAL;

        public Product(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this._name = name;
            this._description = description;
            this._price = price;
            this._quantity = quantity;
            this._category = category;
            this.StoreId = StoreId;
            this.productDAL = new ProductDAL(Id, StoreId, Name, Description, Price, Quantity, Category);
        }

        public Product(ProductDAL productDAL)
        {
            this.Id = productDAL.Id;
            this._name = productDAL.Name;
            this._description = productDAL.Description;
            this._price = productDAL.Price;
            this._quantity = productDAL.Quantity;
            this._category = productDAL.Category;
            this.StoreId = productDAL.Store;
            this.productDAL = productDAL;
        }

        public ProductDAL ToDAL()
        {
            return productDAL;
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
