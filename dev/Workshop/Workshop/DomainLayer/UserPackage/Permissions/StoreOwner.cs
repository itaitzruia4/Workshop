using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using static Workshop.DomainLayer.UserPackage.Permissions.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreOwner: StoreRole
    {
        List<StoreRole> nominees;
        public StoreOwner(int storeId): base(storeId)
        {
            this.nominees = new List<StoreRole>();
            actions.Add(Action.AddProduct);
            actions.Add(Action.RemoveProduct);
            actions.Add(Action.ChangeProductName);
            actions.Add(Action.ChangeProductPrice);
            actions.Add(Action.ChangeProductQuantity);
            actions.Add(Action.GetWorkersInformation);
        }
    }
}
