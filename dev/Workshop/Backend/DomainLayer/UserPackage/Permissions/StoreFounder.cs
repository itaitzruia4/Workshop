using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreFounder: StoreOwner
    {
        public StoreFounder(int storeId) : base(storeId)
        {
            actions.Add(Action.CloseStore);
        }
    }
}
