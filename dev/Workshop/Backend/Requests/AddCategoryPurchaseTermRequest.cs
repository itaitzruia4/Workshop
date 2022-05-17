namespace API.Requests
{
    public class AddCategoryPurchaseTermRequest : StoreRequest
    {
        public string Term { get; set; }
        public string Category { get; set; }
    }
}
