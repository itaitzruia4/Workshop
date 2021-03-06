using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market.Discounts;
using Workshop.DataLayer.DataObjects.Market.Purchases;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects.Market
{
    public class Store : DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public bool Open { get; set; }
        public string StoreName { get; set; }
        public DiscountPolicy DiscountPolicy { get; set; }
        public PurchasePolicy PurchasePolicy { get; set; }
        public List<Product> Products { get; set; }
        public List<Member> Owners { get; set; }

        public Store()
        {
            Products = new List<Product>();
            Owners = new List<Member>();
        }
        public Store(int id, bool open, string storeName, DiscountPolicy discountPolicy, PurchasePolicy purchasePolicy, List<Product> products, List<Member> owners)
        {
            Id = id;
            Open = open;
            StoreName = storeName;
            DiscountPolicy = discountPolicy;
            PurchasePolicy = purchasePolicy;
            Products = products;
            Owners = owners;
        }
    }
}
