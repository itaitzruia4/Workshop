using System.Collections.Generic;

namespace Workshop.DomainLayer.MarketPackage
{
    public class StoreDTO
    {
        public bool open { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public Dictionary<int, Product> products { get; set; }

        public StoreDTO(int id, string name, Dictionary<int, Product> products, bool open)
        {
            this.id = id;
            this.name = name;
            this.products = products;
            this.open = true;
        }
    }
}