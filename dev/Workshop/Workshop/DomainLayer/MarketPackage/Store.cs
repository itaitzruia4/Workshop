using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    class Store
    {
        private int id;
        private Dictionary<int, Product> products;

        public Store(int id)
        {
            this.id = id;
            products = new Dictionary<int, Product>();
        }

        public int getID()
        {
            return this.id;
        }

        internal void AddProduct(int productID)
        {
            throw new NotImplementedException();
        }

        internal void RemoveProduct(int productID)
        {
            throw new NotImplementedException();
        }
    }
}
