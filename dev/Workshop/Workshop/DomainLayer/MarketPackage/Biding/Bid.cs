using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer.MarketPackage.Biding
{
    public class Bid
    {
        public Product product { get; set; }
        public int price { get; set; }
        public string presenter_name { get; set; }
        public CreditCard cc { get; set; }
        public SupplyAddress address { get; set; }
        public HashSet<Member> owner_votes { get; set; }
        public HashSet<Member> owners;
        public Bid(Product product, int price, string presenter_name,HashSet<Member> owners, CreditCard cc, SupplyAddress address)
        {
            this.product = product;
            this.price = price;
            this.presenter_name = presenter_name;
            this.owners = owners;
            this.cc = cc;
            this.address = address;
        }

        public bool AddOwnerVote(Member owner_name)
        {
            if(owners.Contains(owner_name))
            {
                owner_votes.Add(owner_name);
                if(owner_votes.Count == owners.Count)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
