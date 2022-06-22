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
        public int BidId { get; set; }
        public int StoreId { get; set; }
        public Product Product { get; set; }
        public double OfferedPrice { get; set; }
        public string OfferingMembername { get; set; }
        public HashSet<Member> OwnerVotes { get; set; }
        public Bid(int bidId, int storeId, Product product, double price, string presenter_name)
        {
            BidId = bidId;
            StoreId = storeId;
            Product = product;
            OfferedPrice = price;
            OfferingMembername = presenter_name;
            OwnerVotes = new HashSet<Member>();
        }

        public int AddOwnerVote(Member owner_name)
        {
            OwnerVotes.Add(owner_name);
            return OwnerVotes.Count;
        }
    }
}
