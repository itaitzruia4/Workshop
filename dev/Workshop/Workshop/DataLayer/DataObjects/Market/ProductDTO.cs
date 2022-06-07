using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ProductDTO: DALObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public int StoreId { get; set; }

        public ProductDTO()
        {
        }

        public ProductDTO(string name, string description, double price, int quantity, string category, int storeId)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
            StoreId = storeId;
        }
    }
}
