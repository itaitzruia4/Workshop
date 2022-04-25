namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingBagDTO
    {
        public string storeId { get; set; }
        public List<ProductDTO> products { get; set; }

        public ShoppingBagDTO(int storeId,List<ProductDTO> products)
        {
            this.storeId = store;
            products = products;
        }
    }
}