using System;
using ProductDAL = Workshop.DataLayer.DataObjects.Market.Product;
using DALObject = Workshop.DataLayer.DALObject;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Product : IPersistentObject<ProductDAL>
    {
        public int Id { get; set; }
        public string Name { get => Name; set => setName(Name); }
        public double Price { get => Price; set => setPrice(Price); }
        public int Quantity { get => Quantity; set => setQuantity(Quantity); }
        public string Description { get => Name; set => setDescription(Description); }
        public string Category { get => Category; set => setCategory(Category); }
        public int StoreId { get; set; }

        private ProductDAL productDAL;

        public Product(int id, string name, string description, double price, int quantity, string category, int StoreId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.Category = category;
            this.StoreId = StoreId;
            this.productDAL = new ProductDAL(Id, StoreId, Name, Description, Price, Quantity, Category);
        }

        public Product(ProductDAL productDAL)
        {
            this.Id = productDAL.Id;
            this.Name = productDAL.Name;
            this.Description = productDAL.Description;
            this.Price = productDAL.Price;
            this.Quantity = productDAL.Quantity;
            this.Category = productDAL.Category;
            this.StoreId = productDAL.Store;
            this.productDAL = productDAL;
        }

        private void setName(string Name)
        {
            this.Name = Name;
            productDAL.Name = Name;
            DataHandler.getDBHandler().update(productDAL);
        }

        private void setPrice(double Price)
        {
            this.Price = Price;
            productDAL.Price = Price;
            DataHandler.getDBHandler().update(productDAL);
        }

        private void setQuantity(int Quantity)
        {
            this.Quantity = Quantity;
            productDAL.Quantity = Quantity;
            DataHandler.getDBHandler().update(productDAL);
        }

        private void setDescription(string Description)
        {
            this.Description = Description;
            productDAL.Description = Description;
            DataHandler.getDBHandler().update(productDAL);
        }

        private void setCategory(string Category)
        {
            this.Category = Category;
            productDAL.Category = Category;
            DataHandler.getDBHandler().update(productDAL);
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
