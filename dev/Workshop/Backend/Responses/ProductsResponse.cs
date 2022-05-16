using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ProductsResponse
    {
        public List<Product> Products;
        public string Error;

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
