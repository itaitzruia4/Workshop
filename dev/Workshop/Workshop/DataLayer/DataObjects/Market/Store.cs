using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class Store
    {
        public int Id { get; set; }
        public bool Open { get; set; }
        public string StoreName { get; set; }
        public DiscountPolicy DiscountPolicy { get; set; }
        public PurchasePolicy PurchasePolicy { get; set; }
        public List<Product> Products { get; set; }
        /*public override string ToString()
        {
            return $"{Id} {Open} {StoreName} {DiscountPolicy} {PurchasePolicy}";
        }*/
    }
}
