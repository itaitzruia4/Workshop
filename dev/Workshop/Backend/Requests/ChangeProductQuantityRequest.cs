namespace API.Requests
{
    public class ChangeProductQuantityRequest : ProductStoreRequest
    {
        public int NewQuantity { get; set; }
    }
}
