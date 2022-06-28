using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;
using StatisticsInformation = Workshop.ServiceLayer.ServiceObjects.StatisticsInformation;
namespace Workshop.DomainLayer.UserPackage
{
    public class UserCountInDate
    {
        private HashSet<User> Guests;
        private HashSet<Member> Members;
        private HashSet<Member> StoreManagers;
        private HashSet<Member> StoreOwners;
        private HashSet<Member> MarketManagers;
        private string Date;

        public UserCountInDate(DateTime date)
        {
            Date = date.ToShortDateString();
            Guests = new HashSet<User>();
            Members = new HashSet<Member>();
            StoreManagers = new HashSet<Member>();
            StoreOwners = new HashSet<Member>();
            MarketManagers = new HashSet<Member>();
        }

        public int GetGuests()
        {
            return Guests.Count;
        }

        public int GetMembers()
        {
            return Members.Count;
        }
        public int GetStoreManagers()
        {
            return StoreManagers.Count;
        }
        public int GetStoreOwners()
        {
            return StoreOwners.Count;
        }
        public int GetMarketManagers()
        {
            return MarketManagers.Count;
        }
        public string GetDate()
        {
            return Date;
        }

        public void IncreaseCount(User u)
        {
            string hrank = HighestRank(u);
            if (hrank.Equals("guest"))
            {
                Guests.Add(u);
                return;
            }
            Member m = (Member)u;
            switch (hrank)
            {
                case "member":
                    Members.Add(m);
                    if (Guests.Contains(m))
                    {
                        Guests.Remove(m);
                    }
                    break;
                case "manager":
                    StoreManagers.Add(m);
                    if (Members.Contains(m))
                    {
                        Members.Remove(m);
                    }
                    if (Guests.Contains(m))
                    {
                        Guests.Remove(m);
                    }
                    break;
                case "owner":
                    StoreOwners.Add(m);
                    if (StoreManagers.Contains(m))
                    {
                        StoreManagers.Remove(m);
                    }
                    if (Members.Contains(m))
                    {
                        Members.Remove(m);
                    }
                    if (Guests.Contains(m))
                    {
                        Guests.Remove(m);
                    }
                    break;
                case "market":
                    MarketManagers.Add(m);
                    if (StoreOwners.Contains(m))
                    {
                        StoreOwners.Remove(m);
                    }
                    if (StoreManagers.Contains(m))
                    {
                        StoreManagers.Remove(m);
                    }
                    if (Members.Contains(m))
                    {
                        Members.Remove(m);
                    }
                    if (Guests.Contains(m))
                    {
                        Guests.Remove(m);
                    }
                    break;
            }
        }

        private string HighestRank(User u)
        {
            if (!(u is Member))
            {
                return "guest";
            }
            bool MANAGER = false;
            bool OWNER = false;
            bool MARKET = false;
            foreach (Role role in ((Member)u).GetAllRoles())
            {
                if (!MARKET && role is MarketManager)
                {
                    MARKET = true;
                }
                else if (!OWNER && (role is StoreOwner || role is StoreFounder))
                {
                    OWNER = true;
                }
                else if (!MANAGER && role is StoreManager)
                {
                    MANAGER = true;
                }
            }
            if (MARKET) return "market";
            if (OWNER) return "owner";
            if (MANAGER) return "manager";
            return "member";
        }
    }
}
