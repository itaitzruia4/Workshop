using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class SupplyAddress: DALObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }

        public SupplyAddress()
        {
        }

        public SupplyAddress(string name, string address, string city, string country, string zip)
        {
            Name = name;
            Address = address;
            City = city;
            Country = country;
            Zip = zip;
        }
    }
}
