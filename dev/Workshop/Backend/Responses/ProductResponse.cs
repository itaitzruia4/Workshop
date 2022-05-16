using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductResponse
    {
        public Product Product;
        public string Error;

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
