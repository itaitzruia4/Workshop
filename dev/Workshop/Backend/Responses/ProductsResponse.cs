using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductsResponse
    {
        public List<Product> Products { get; set; }
        public string Error { get; set; }

        public ProductsResponse(List<Product> products)
        {
            Products = products;
        }
        public ProductsResponse(string error)
        {
            Error = error;
        }
    }
}
