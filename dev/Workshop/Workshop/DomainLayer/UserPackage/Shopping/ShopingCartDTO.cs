namespace Workshop.DomainLayer.UserPackage
{
    class ShoppingCartDTO
    {
        private Dictionary<int,ShoppingBagDTO> shopingBags { get; set; }

        public ShoppingCartDTO(Dictionary<int,ShoppingBagDTO> shopingBags)
        {
            shopingBags = shopingBags;
        }
    }
}