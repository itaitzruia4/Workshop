namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingCartDTO
    {
        private Dictionary<int,ShoppingBagDTO> shoppingBags { get; set; }

        public ShoppingCartDTO(Dictionary<int,ShoppingBagDTO> shoppingBags)
        {
            shoppingBags = shoppingBags;
        }
        public int getPrice()
        {
            int price = 0;
            foreach (ShoppingBagDTO bag in shoppingBags.Values)
            {
                foreach (ProductDTO product in bag.products)
                {
                    price += product.Price;
                }
            }
            return price;
        }
    }
}