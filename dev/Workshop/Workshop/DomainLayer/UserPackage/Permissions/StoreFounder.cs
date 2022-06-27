using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer;
using StoreOwnerDAl = Workshop.DataLayer.DataObjects.Members.Role;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using DataHandler = Workshop.DataLayer.DataHandler;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;


namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreFounder: StoreOwner
    {
        public StoreFounder(int storeId) : base(storeId)
        {
            actions.Add(Action.CloseStore);

            roleDAL.RoleType = "StoreFounder";
            foreach (var action in actions)
                roleDAL.Actions.Add(new ActionDAL((int)action));

            DataHandler.getDBHandler().update(roleDAL);
        }

        public StoreFounder(RoleDAL roleDAL) : base(roleDAL)
        {}

        public override StoreOwnerDAl ToDAL()
        {
            return base.ToDAL();
        }
    }
}
