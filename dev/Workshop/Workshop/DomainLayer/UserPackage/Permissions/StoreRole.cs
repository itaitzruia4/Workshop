using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage.Permissions
{

    class StoreRole: Role
    {
        protected int storeId;
        protected HashSet<Action> store_actions;

        public StoreRole(int storeId, HashSet<Action> store_actions)
        {
            this.storeId = storeId;
            this.store_actions = store_actions;
        }

        public override bool IsAuthorized(int storeID, Action action)
        {
            if(this.storeId != storeID)
                return false;
            return store_actions.Contains(action);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;
            if(obj.GetType() != typeof(StoreRole))
                return false;
            return storeId == ((StoreRole)obj).storeId;
        }
    }
}
