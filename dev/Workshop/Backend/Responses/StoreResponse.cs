using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreResponse
    {
        public Store Store;
        public string Error;

        public StoreResponse(Store st)
        {
            Store = st;
        }
        public StoreResponse(string error)
        {
            Error = error;
        }

    }
}
