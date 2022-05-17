namespace API.Requests
{
    public class AddToCartRequest : EditCartRequest
    {
        public int StoreId { get; set; }
    }
}
