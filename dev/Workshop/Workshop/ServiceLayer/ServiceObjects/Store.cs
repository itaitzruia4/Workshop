using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainStore = Workshop.DomainLayer.MarketPackage.Store;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Store
    {
        public IReadOnlyDictionary<int, Product> Products { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public bool Open { get; set; }

        internal Store(IReadOnlyDictionary<int, Product> products, string name)
        {
            Products = products;
            Name = name;
        }

        internal Store(DomainStore domainStore)
        {
            IReadOnlyDictionary<int, DomainProduct> domainProducts = domainStore.GetProducts();
            Dictionary<int, Product> products = new Dictionary<int, Product>();
            foreach (int productId in domainProducts.Keys)
            {
                products[productId] = new Product(domainProducts[productId]);
            }
            Products = products;
            Name = domainStore.GetStoreName();
            StoreId = domainStore.GetId();
            Open = domainStore.IsOpen();
        }
    }
}
