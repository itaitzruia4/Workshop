using System;
using System.Collections.Generic;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class ShoppingBagDTO
    {
        public int storeId { get; set; }
        public List<ProductDTO> products { get; set; }

        public ShoppingBagDTO(int storeId,List<ProductDTO> products)
        {
            this.storeId = storeId;
            this.products = products;
        }

        internal bool IsEmpty()
        {
            return products.Count == 0;
        }
    }
}