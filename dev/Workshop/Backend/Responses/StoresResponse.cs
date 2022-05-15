namespace API.Responses
{
    public class StoresResponse
    {
        public List<StoreResponse> Stores;
        public string Error;
        
        public StoresResponse(List<StoreResponse> stores)
        {
            Stores = stores;
        }
        public StoresResponse(string error)
        {
            Error = error;
        }
    }
}
