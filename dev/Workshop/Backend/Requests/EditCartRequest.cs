namespace API.Requests
{
    public class EditCartRequest : BaseRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
