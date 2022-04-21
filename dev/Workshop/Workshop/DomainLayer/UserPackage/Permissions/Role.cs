using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    abstract class Role
    {
        protected HashSet<Action> actions;
        public Role() 
        {
            actions = new HashSet<Action>();
        }

        public bool IsAuthorized(Action action)
        {
            return actions.Contains(action);
        }

        public abstract bool IsAuthorized(int storeID, Action action);
        
    }
}
