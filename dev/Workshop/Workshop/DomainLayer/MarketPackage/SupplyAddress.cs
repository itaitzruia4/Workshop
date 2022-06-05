namespace Workshop.DomainLayer.MarketPackage
{
    public class SupplyAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
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