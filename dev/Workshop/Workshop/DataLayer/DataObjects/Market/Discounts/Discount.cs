using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class Discount : DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string discountJson { get; set; }
        public Discount()
        {
            this.Id = nextId;
            nextId++;
        }

        public Discount(string discountJson)
        {
            this.discountJson = discountJson;
            this.Id = nextId;
            nextId++;
        }
    }
}
