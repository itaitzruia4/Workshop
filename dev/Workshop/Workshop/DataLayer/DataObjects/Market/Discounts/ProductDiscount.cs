using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class ProductDiscount: DALObject
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Discount Discount { get; set; }

        public ProductDiscount()
        {
            this.Id = nextId;
            nextId++;
        }

        public ProductDiscount(int ProductId, Discount Discount)
        {
            this.Id = nextId;
            nextId++;
            this.ProductId = ProductId;
            this.Discount = Discount;
        }
    }
}
