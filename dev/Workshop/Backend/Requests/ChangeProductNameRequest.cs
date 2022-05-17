namespace API.Requests
{
    public class ChangeProductNameRequest : ProductStoreRequest
    {
        public string NewName { get; set; }
    }
}
