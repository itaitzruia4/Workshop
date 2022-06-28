namespace API.Requests
{
    public class ChangeProductCategoryRequest : ProductStoreRequest
    {
        public string NewCategory { get; set; }
    }
}
