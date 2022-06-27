using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer;
using Workshop.DomainLayer.MarketPackage;
using static Workshop.DomainLayer.UserPackage.Permissions.Role;
using StoreOwnerDAl = Workshop.DataLayer.DataObjects.Members.Role;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using DataHandler = Workshop.DataLayer.DataHandler;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreOwner: StoreRole
    {
        public StoreOwner(int storeId): base(storeId)
        {
            actions.Add(Action.AddProduct);
            actions.Add(Action.RemoveProduct);
            actions.Add(Action.ChangeProductName);
            actions.Add(Action.ChangeProductDescription);
            actions.Add(Action.ChangeProductPrice);
            actions.Add(Action.ChangeProductQuantity);
            actions.Add(Action.GetWorkersInformation);
            actions.Add(Action.AddPermissionToStoreManager);
            actions.Add(Action.RemovePermissionFromStoreManager);
            actions.Add(Action.GetStoreOrdersList);
            actions.Add(Action.ViewClosedStore);
            actions.Add(Action.NominateStoreOwner);
            actions.Add(Action.NominateStoreManager);
            actions.Add(Action.AddDiscount);
            actions.Add(Action.AddPurchaseTerm);
            actions.Add(Action.ViewStorePurchaseHistory);
            actions.Add(Action.ChangeProductCategory);

            roleDAL.RoleType = "StoreOwner";
            foreach(var action in actions)
                roleDAL.Actions.Add(new ActionDAL((int)action));

            DataHandler.getDBHandler().save(roleDAL);
        }

        public StoreOwner(RoleDAL roleDAL) : base(roleDAL)
        { }

        public override StoreOwnerDAl ToDAL()
        {
            return base.ToDAL();
        }
    }
}
