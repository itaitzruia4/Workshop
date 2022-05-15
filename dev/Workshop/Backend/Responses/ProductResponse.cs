using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double BasePrice { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string Error { get; set; }

        public ProductResponse(Product p)
        {
            ProductId = p.Id;
            Name = p.Name;
            BasePrice = p.BasePrice;
            Description = p.Description;
            Quantity = p.Quantity;
            Category = p.Category;
        }

        public ProductResponse(string error)
        {
            Error = error;
        }
    }
}
