using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class Role
    {
        // Store ID -1 means general actions (not store related)
        protected Dictionary<int, HashSet<Action>> stores_to_actions;
        public Role() {
            stores_to_actions = new Dictionary<int, HashSet<Action>>();
        }

        public Boolean IsAuthorized(int storeID,Action action)
        {
            if(!stores_to_actions.ContainsKey(storeID))
                return false;
            return stores_to_actions[storeID].Contains(action);
        }
    }
}
