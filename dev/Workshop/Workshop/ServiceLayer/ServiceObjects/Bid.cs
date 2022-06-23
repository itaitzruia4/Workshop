using System.Collections.Generic;
using System.Linq;
using DomainBid = Workshop.DomainLayer.MarketPackage.Biding.Bid;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Bid
    {
        public int BidId { get; set; }
        public int StoreId { get; set; }
        public Product Product { get; set; }
        public double OfferedPrice { get; set; }
        public string OfferingMembername { get; set; }
        public List<Member> OwnerVotes { get; set; }

        public Bid(DomainBid db)
        {
            BidId = db.BidId;
            StoreId = db.StoreId;
            Product = new Product(db.Product);
            OfferedPrice = db.OfferedPrice;
            OfferingMembername = db.OfferingMembername;
            OwnerVotes = db.OwnerVotes.Select(dm => new Member(dm)).ToList();
        }
    }
}
