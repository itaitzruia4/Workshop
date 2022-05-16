namespace API.Responses
{
    public class PriceResponse
    {
        public double Price { get; set; }
        public string Error { get; set; }
        public PriceResponse(double price)
        {
            Price = price;
        }
        public PriceResponse(string error)
        {
            Error = error;
        }

    }
}
