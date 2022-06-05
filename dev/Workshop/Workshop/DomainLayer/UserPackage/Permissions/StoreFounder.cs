using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer;
using StoreOwnerDAl = Workshop.DataLayer.DataObjects.Members.StoreOwner;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreFounder: StoreOwner
    {
        public StoreFounder(int storeId) : base(storeId)
        {
            actions.Add(Action.CloseStore);
        }

        public override DALObject ToDAL()
        {
            return (StoreOwnerDAl) base.ToDAL();
        }
    }
}
