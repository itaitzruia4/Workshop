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
        public List<ProductDiscount> products_discounts { get; set; }
        public List<CategoryDiscount> category_discounts { get; set; }
        public Discount store_discount { get; set; }

        public DiscountPolicy()
        {
            this.Id = nextId;
            nextId++;
            this.products_discounts = new List<ProductDiscount>();
            this.category_discounts = new List<CategoryDiscount>();
        }

        public DiscountPolicy(List<ProductDiscount> products_discounts, List<CategoryDiscount> category_discounts, Discount store_discount)
        {
            this.products_discounts = products_discounts;
            this.category_discounts = category_discounts;
            this.store_discount = store_discount;
            this.Id = nextId;
            nextId++;
        }
    }
}
