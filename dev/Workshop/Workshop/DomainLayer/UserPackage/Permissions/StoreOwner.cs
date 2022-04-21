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
        public StoreOwner(int storeId): base(storeId, new HashSet<Action>())
        {
            store_actions.Add(Action.AddProduct);
            store_actions.Add(Action.RemoveProduct);
            store_actions.Add(Action.ChangeProductName);
            store_actions.Add(Action.ChangeProductPrice);
            store_actions.Add(Action.ChangeProductQuantity);
        }
    }
}
