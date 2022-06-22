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
        private int Guests;
        private int Members;
        private int StoreManagers;
        private int StoreOwners;
        private int MarketManagers;
        private string Date;

        public UserCountInDate(DateTime date)
        {
            Date = date.ToShortDateString();
            Guests = 0;
            Members = 0;
            StoreManagers = 0;
            StoreOwners = 0;
            MarketManagers = 0;
        }

        public int GetGuests()
        {
            return Guests;
        }

        public int GetMembers()
        {
            return Members;
        }
        public int GetStoreManagers()
        {
            return StoreManagers;
        }
        public int GetStoreOwners()
        {
            return StoreOwners;
        }
        public int GetMarketManagers()
        {
            return MarketManagers;
        }
        public string GetDate()
        {
            return Date;
        }

        public void IncreaseCount(User u)
        {
            IncreaseCount(HighestRank(u));
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

        private void IncreaseCount(string type)
        {
            switch (type)
            {
                case "guest":
                    Interlocked.Increment(ref Guests);
                    break;
                case "member":
                    Interlocked.Increment(ref Members);
                    break;
                case "manager":
                    Interlocked.Increment(ref StoreManagers);
                    break;
                case "owner":
                    Interlocked.Increment(ref StoreOwners);
                    break;
                case "market":
                    Interlocked.Increment(ref MarketManagers);
                    break;
                default:
                    throw new ArgumentException($"A user type {type} is not recognized!");
            }
        }
    }
}
