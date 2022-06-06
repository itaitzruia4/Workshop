using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer;
using DALObject = Workshop.DataLayer.DALObject;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public abstract class Role : IPersistentObject
    {
        protected HashSet<Action> actions;
        public Role() 
        {
            actions = new HashSet<Action>();
        }

        public virtual DALObject ToDAL()
        {
            List<ActionDAL> actionsDAL = new List<ActionDAL>();
            foreach (Action action in actions)
            {
                actionsDAL.Add(new ActionDAL(((int)action)));
            }
            return new RoleDAL(1, actionsDAL,"");
        }

        public IReadOnlyCollection<Action> GetAllActions()
        {
            return new HashSet<Action>(actions);
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
