using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class StoreResponse
    {
        public IReadOnlyDictionary<int, Product> Products { get; set; }
        public string StoreName { get; set; }
        public int StoreId { get; set; }
        public bool Open { get; set; }
        public string Error { get; set; }

        public StoreResponse(string error)
        {
            Error = error;
        }

        public StoreResponse(IReadOnlyDictionary<int, Product> products, string storeName, int storeId, bool open)
        {
            Products = products;
            StoreName = storeName;
            StoreId = storeId;
            Open = open;
        }

        public StoreResponse(Store st)
        {
            Products = st.Products;
            StoreName = st.Name;
            StoreId = st.StoreId;
            Open = st.Open;
        }
    }
}
