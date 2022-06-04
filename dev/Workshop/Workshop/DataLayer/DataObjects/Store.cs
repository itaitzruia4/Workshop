using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects
{
    public class Store
    {
        public int Id { get; set; }
        public bool Open { get; set; }
        public string StoreName { get; set; }
        public string DiscountPolicy { get; set; }
        public string PurchasePolicy { get; set; }

        public override string ToString()
        {
            return $"{Id} {Open} {StoreName} {DiscountPolicy} {PurchasePolicy}";
        }
    }
}
