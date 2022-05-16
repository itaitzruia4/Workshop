using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class ShoppingCartResponse
    {
        ShoppingCart ShoppingCart;
        string Error;
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
