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
        public List<Product> Products { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public bool Open { get; set; }

        internal Store(DomainStore domainStore)
        {
            Products = domainStore.GetProducts().Values.Select(p => new Product(p)).ToList();
            Name = domainStore.GetStoreName();
            StoreId = domainStore.GetId();
            Open = domainStore.IsOpen();
        }
    }
}
