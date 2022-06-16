using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class CompositeDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Discount firstDiscount { get; set; }
        public Discount secondDiscount { get; set; }

        public CompositeDiscount(Discount firstDiscount, Discount secondDiscount)
        {
            this.firstDiscount = firstDiscount;
            this.secondDiscount = secondDiscount;
        }
    }
}
