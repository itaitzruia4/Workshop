namespace API.Requests
{
    public class AddToCartRequest : BaseRequest
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
    }
}
