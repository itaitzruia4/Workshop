namespace Workshop.DomainLayer.MarketPackage
{
    public class ShopingBagProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ShopingBagProduct(int id, string name, double price, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }
    }
}