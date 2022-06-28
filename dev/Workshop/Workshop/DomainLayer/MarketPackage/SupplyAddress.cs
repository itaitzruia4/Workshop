using SupplyAddressDAL = Workshop.DataLayer.DataObjects.Market.SupplyAddress;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.MarketPackage
{
    public class SupplyAddress: IPersistentObject<SupplyAddressDAL>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public SupplyAddressDAL SupplyAddressDAL { get; set; }

        public SupplyAddress(string name, string address, string city, string country, string zip)
        {
            Name = name;
            Address = address;
            City = city;
            Country = country;
            Zip = zip;
            this.SupplyAddressDAL = new SupplyAddressDAL(name, address, city, country, zip);
            DataHandler.Instance.Value.save(SupplyAddressDAL);
        }

        public SupplyAddress(SupplyAddressDAL supplyAddressDAL)
        {
            this.Name = supplyAddressDAL.Name;
            this.Address = supplyAddressDAL.Address;
            this.City = supplyAddressDAL.City;
            this.Country = supplyAddressDAL.City;
            this.Zip = supplyAddressDAL.Zip;
            this.SupplyAddressDAL = supplyAddressDAL;
        }

        public SupplyAddressDAL ToDAL()
        {
            return this.SupplyAddressDAL;
        }
    }
}