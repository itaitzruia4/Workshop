using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    abstract class Role
    {
        protected HashSet<Action> general_actions;
        public Role() 
        {
            general_actions = new HashSet<Action>();
        }

        public bool IsAuthorized(Action action)
        {
            return general_actions.Contains(action);
        }

        public abstract Boolean IsAuthorized(int storeID, Action action);
        
    }
}
