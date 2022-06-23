using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class MarketManager : Role
    {
        public MarketManager()
        {
            actions.Add(Action.ViewClosedStore);
            actions.Add(Action.CancelMember);
            actions.Add(Action.GetMarketStatistics);
            actions.Add(Action.GetMembersOnlineStats);
            actions.Add(Action.ViewStorePurchaseHistory);
        }
    }
}
