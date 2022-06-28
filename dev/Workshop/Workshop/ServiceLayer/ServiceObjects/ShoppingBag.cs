using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShoppingBag = Workshop.DomainLayer.UserPackage.Shopping.ShoppingBagDTO;
using DomainProduct = Workshop.DomainLayer.MarketPackage.ProductDTO;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class ShoppingBag
    {
        public int StoreId { get; set; }
        public List<Product> Products { get; set; }

        public ShoppingBag(DomainShoppingBag shoppingBag)
        {
            this.StoreId = shoppingBag.storeId;
            this.Products = shoppingBag.products.Select(x => new Product(x)).ToList();
        }
    }
}
