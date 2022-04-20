using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using static Workshop.DomainLayer.UserPackage.Permissions.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class StoreOwner: StoreRole
    {
        public StoreOwner(Store store)
        {
            HashSet<Action> actions = new HashSet<Action>();
            this.stores_to_actions.Add(store.getID(), actions);
            actions.Add(Action.AddProduct);
            actions.Add(Action.RemoveProduct);
            actions.Add(Action.ChangeProductName);
            actions.Add(Action.ChangeProductPrice);
            actions.Add(Action.ChangeProductQuantity);
        }
    }
}
