using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class MarketManager: Role
    {
        public MarketManager()
        {
            actions.Add(Action.ViewClosedStore);
        }
        public override bool IsAuthorized(int storeID, Action action)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType() != typeof(MarketManager);
        }
    }
}
