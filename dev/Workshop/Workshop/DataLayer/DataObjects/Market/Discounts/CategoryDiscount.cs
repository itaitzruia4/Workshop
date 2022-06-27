using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class CategoryDiscount: DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Discount Discount { get; set; }

        public CategoryDiscount()
        {
            this.Id = nextId;
            nextId++;
        }

        public CategoryDiscount(string Name, Discount Discount)
        {
            this.Id = nextId;
            nextId++;
            this.Name = Name;
            this.Discount = Discount;
        }
    }
}
