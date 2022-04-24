using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage
{
    public class StoreDTO
    {
        public bool IsOpen { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<int, Product> Products { get; set; }

        public StoreDTO(int id, string name, Dictionary<int, Product> products, bool open)
        {
            this.Id = id;
            this.Name = name;
            this.Products = products;
            this.IsOpen = true;
        }
    }
}