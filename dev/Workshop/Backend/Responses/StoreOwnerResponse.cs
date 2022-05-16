using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreOwnerResponse
    {
        public StoreOwner StoreOwner;
        public string Error;

        public StoreOwnerResponse(StoreOwner so)
        {
            StoreOwner = so;
        }
        public StoreOwnerResponse(string error)
        {
            Error = error;
        }
    }
}
