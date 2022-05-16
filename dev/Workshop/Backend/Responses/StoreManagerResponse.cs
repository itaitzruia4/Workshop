using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreManagerResponse
    {
        public StoreManager StoreManager;
        public string Error;

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
