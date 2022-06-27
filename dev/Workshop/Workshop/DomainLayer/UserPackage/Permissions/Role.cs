using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer;
using DALObject = Workshop.DataLayer.DALObject;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public abstract class Role : IPersistentObject<RoleDAL>
    {
        protected HashSet<Action> actions;
        protected RoleDAL roleDAL;
        public Role() 
        {
            actions = new HashSet<Action>();
            roleDAL = new RoleDAL(-1, new List<ActionDAL>(), "Role");
        }

        public Role(RoleDAL roleDAL)
        {
            actions = new HashSet<Action>();
            this.roleDAL = roleDAL;
            foreach (ActionDAL action in roleDAL.Actions)
                actions.Add((Action)action.ActionType);
        }

        public static Role createRole(RoleDAL roleDAL)
        {
            switch (roleDAL.RoleType)
            {
                case "MarketManager":
                    return new MarketManager(roleDAL);
                case "StoreFounder":
                    return new StoreFounder(roleDAL);
                case "StoreManager":
                    return new StoreManager(roleDAL);
                case "StoreOwner":
                    return new StoreOwner(roleDAL);
                //case "StoreRole":
                case "Role":
                    throw new ArgumentException("Can't reate such role");
                default:
                    throw new ArgumentException("No such role type");
            }
        }


        public virtual RoleDAL ToDAL()
        {
            return roleDAL;
        }

        public List<Action> GetAllActions()
        {
            return new List<Action>(actions);
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
            roleDAL.Actions.Add(new ActionDAL(((int)action)));
            DataHandler.Instance.Value.update(roleDAL);
        }
        public void RemoveAction(Action action)
        {
            if (!actions.Contains(action))
                throw new ArgumentException("This member already does not have the requested permission.");
            actions.Remove(action);

            ActionDAL toRemove = null;
            foreach(ActionDAL actionDAL in roleDAL.Actions) 
            {
                if (actionDAL.ActionType == (int)action)
                {
                    toRemove = actionDAL;
                    break;
                }
            }
            roleDAL.Actions.Remove(toRemove);
            DataHandler.Instance.Value.update(roleDAL);
        }

    }
}
