using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreManagerResponse
    {
        public StoreManager StoreManager { get; set; }
        public string Error { get; set; }

        public StoreManagerResponse(StoreManager sm)
        {
            StoreManager = sm;
        }
        public StoreManagerResponse(string error)
        {
            Error = error;
        }
    }
}
