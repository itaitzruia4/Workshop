using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductResponse
    {
        public Product Product { get; set; }
        public string Error { get; set; }

        public ProductResponse(Product p)
        {
            Product = p;
        }

        public ProductResponse(string error)
        {
            Error = error;
        }
    }
}
