namespace Workshop.DomainLayer.UserPackage
{
    class ShoppingBagDTO
    {
        public string storeId { get; set; }
        public Dictionary<int, List<ProductDTO>> products { get; set; }

        public ShoppingBagDTO(int storeId,Dictionary<int, List<ProductDTO>> products)
        {
            this.storeId = store;
            shoppingBags = products;
        }
    }
}