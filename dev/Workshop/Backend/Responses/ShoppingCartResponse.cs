using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ShoppingCartResponse
    {
        ShoppingCart ShoppingCart { get; set; }
        string Error { get; set; }
        public ShoppingCartResponse(ShoppingCart sc)
        {
            ShoppingCart = sc;
        }
        public ShoppingCartResponse(string error)
        {
            Error = error;
        }
    }
}
