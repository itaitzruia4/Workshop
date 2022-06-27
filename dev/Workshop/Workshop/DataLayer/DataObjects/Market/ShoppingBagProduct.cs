using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class ShoppingBagProduct : DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public ShoppingBagProduct()
        {
            this.Id = nextId;
            nextId++;
        }

        public ShoppingBagProduct(string name, double price, int quantity, string description, string category)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
            Category = category;
            this.Id = nextId;
            nextId++;
        }
    }
}
