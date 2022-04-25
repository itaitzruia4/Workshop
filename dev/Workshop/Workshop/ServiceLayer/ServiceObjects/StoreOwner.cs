using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainStoreOwner = Workshop.DomainLayer.UserPackage.Permissions.StoreOwner;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class StoreOwner
    {
        public readonly int StoreId;

        public StoreOwner(DomainStoreOwner domainOwner)
        {
            this.StoreId = domainOwner.StoreId;
        }
    }
}
