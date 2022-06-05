namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Product
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string Name { get; set; }
        public double BasePrice { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        internal Product(dynamic domainProduct)
        {
            Id = domainProduct.Id;
            Name = domainProduct.Name;
            BasePrice = domainProduct.Price;
            Description = domainProduct.Description;
            Quantity = domainProduct.Quantity;
            Category = domainProduct.Category;
            StoreId = domainProduct.StoreId;
        }
    }
}
