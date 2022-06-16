using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ProductDTO: DALObject
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public int StoreId { get; set; }

        public ProductDTO()
        {
            this.Id = nextId;
            nextId++;
        }

        public ProductDTO(string name, string description, double price, int quantity, string category, int storeId)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
            StoreId = storeId;
            this.Id = nextId;
            nextId++;
        }
    }
}
