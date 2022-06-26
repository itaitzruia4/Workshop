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
        public static int nextId = 0;
        private int id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get { return id; } set { id = value; nextId = value + 1; } }
        public int ProductId { get; set; }
        public Discount Discount { get; set; }

        public ProductDiscount()
        {
        }

        public ProductDiscount(int ProductId, Discount Discount)
        {
            this.Id = nextId;
            this.ProductId = ProductId;
            this.Discount = Discount;
        }
    }
}
