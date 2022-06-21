using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class StatisticsInformation
    {
        public string Date { get; set; }
        public int Guests { get; set; }
        public int Members { get; set; }
        public int StoreManagers { get; set; }
        public int StoreOwners { get; set; }
        public int MarketManagers { get; set; }

        public StatisticsInformation(DomainLayer.UserPackage.UserCountInDate ucd)
        {
            Date = ucd.GetDate();
            Guests = ucd.GetGuests();
            Members = ucd.GetMembers();
            StoreManagers = ucd.GetStoreManagers();
            StoreOwners = ucd.GetStoreOwners();
            MarketManagers = ucd.GetMarketManagers();
        }
    }
}
