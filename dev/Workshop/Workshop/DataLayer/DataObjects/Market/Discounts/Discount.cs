using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Discounts
{
    public class Discount : DALObject
    {
        public int Id { get; set; }
        public string discountJson { get; set; }
        public Discount()
        { }

        public Discount(string discountJson)
        {
            this.discountJson = discountJson;
        }
    }
}
