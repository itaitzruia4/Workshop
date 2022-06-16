using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class DiscountPolicy: DALObject
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<Discount> products_discounts { get; set; }
        public List<Discount> category_discounts { get; set; }
        public Discount store_discount { get; set; }

        public DiscountPolicy()
        {
            this.Id = nextId;
            nextId++;
        }

        public DiscountPolicy(List<Discount> products_discounts, List<Discount> category_discounts, Discount store_discount)
        {
            this.products_discounts = products_discounts;
            this.category_discounts = category_discounts;
            this.store_discount = store_discount;
            this.Id = nextId;
            nextId++;
        }
    }
}
