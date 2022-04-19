using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Product
    {
        public readonly int Id;
        public readonly string Name;
        public readonly double BasePrice;
        public readonly string Description;

        internal Product(int id, string name, double basePrice, string description)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            Description = description;
        }
    }
}
