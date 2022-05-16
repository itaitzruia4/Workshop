using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoresResponse
    {
        public List<Store> Stores { get; set; }
        public string Error { get; set; }
        
        public StoresResponse(List<Store> stores)
        {
            Stores = stores;
        }
        public StoresResponse(string error)
        {
            Error = error;
        }
    }
}
