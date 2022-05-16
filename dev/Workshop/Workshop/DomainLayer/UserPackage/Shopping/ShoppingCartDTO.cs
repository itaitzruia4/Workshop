using System;
using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingCartDTO
    {
        public Dictionary<int,ShoppingBagDTO> shoppingBags { get; set; }

        public ShoppingCartDTO(Dictionary<int,ShoppingBagDTO> shoppingBags)
        {
            this.shoppingBags = shoppingBags;
        }
        public double getPrice()
        {
            double price = 0;
            foreach (ShoppingBagDTO bag in shoppingBags.Values)
            {
                foreach (ProductDTO product in bag.products)
                {
                    price += product.Price;
                }
            }
            return price;
        }

        internal bool IsEmpty()
        {
            foreach (ShoppingBagDTO bag in shoppingBags.Values)
            {
                if (!bag.IsEmpty())
                    return false;
            }
            return true;
        }
    }
}