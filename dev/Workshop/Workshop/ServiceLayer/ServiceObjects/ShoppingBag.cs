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
        public int storeId { get; set; }
        public List<Product> products { get; set; }

        public ShoppingBag(int storeId, List<Product> products)
        {
            this.storeId = storeId;
            this.products = products;
        }
        public ShoppingBag(DomainShoppingBag shoppingBag)
        {
            this.storeId = shoppingBag.storeId;
            this.products = new List<Product>();
            foreach(DomainProduct product in shoppingBag.products)
            {
                this.products.Add(new Product(product));
            }
        }
    }
}
