using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer.UserPackage
{
    public class UserCountInDate
    {
        private int Guest_count;
        private int Member_count;
        private int Manager_count;
        private int Owner_count;
        private int Market_count;
        public readonly DateTime Date;

        public UserCountInDate(DateTime date)
        {
            Date = date;
            Guest_count = 0;
            Member_count = 0;
            Manager_count = 0;
            Owner_count = 0;
            Market_count = 0;
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
                    Interlocked.Increment(ref Guest_count);
                    break;
                case "member":
                    Interlocked.Increment(ref Member_count);
                    break;
                case "manager":
                    Interlocked.Increment(ref Manager_count);
                    break;
                case "owner":
                    Interlocked.Increment(ref Owner_count);
                    break;
                case "market":
                    Interlocked.Increment(ref Market_count);
                    break;
                default:
                    throw new ArgumentException($"A user type {type} is not recognized!");
            }
        }

        public dynamic Information()
        {
            dynamic result = new Dictionary<string, dynamic>()
            {
                {"date", Date},
                {"guest", Guest_count },
                {"member", Member_count },
                {"manager", Manager_count },
                {"owner", Owner_count },
                {"market", Market_count }
            };
            return result;
        }
    }
}
