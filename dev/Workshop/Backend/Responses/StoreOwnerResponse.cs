using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreOwnerResponse
    {
        public StoreOwner StoreOwner { get; set; }
        public string Error { get; set; }

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
