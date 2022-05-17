namespace API.Requests
{
    public class ChangeProductPriceRequest : ProductStoreRequest
    {
        public double NewPrice { get; set; }
    }
}
