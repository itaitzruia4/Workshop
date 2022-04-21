using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Store
    {
        public readonly IReadOnlyCollection<string> ProductsNames;
        public readonly string Name;
        public readonly string Owner;

        internal Store(IReadOnlyCollection<string> productsNames, string name, string owner)
        {
            ProductsNames = productsNames;
            Name = name;
            Owner = owner;
        }
    }
}
