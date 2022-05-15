using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductResponse
    {
        public int ProductId;
        public string Name;
        public double BasePrice;
        public string Description;
        public int Quantity;
        public string Category;
        public string Error;

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
