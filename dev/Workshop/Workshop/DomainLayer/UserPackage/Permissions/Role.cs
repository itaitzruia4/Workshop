using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public abstract class Role
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

        public virtual bool IsAuthorized(int storeID, Action action)
        {
            return false;
        }

        public void AddAction(Action action)
        {
            if (actions.Contains(action))
                throw new ArgumentException("This member already have the requested permission.");
            actions.Add(action);
        }
        public void RemoveAction(Action action)
        {
            if (!actions.Contains(action))
                throw new ArgumentException("This member already does not have the requested permission.");
            actions.Remove(action);
        }

    }
}
