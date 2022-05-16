using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainStoreFounder = Workshop.DomainLayer.UserPackage.Permissions.StoreFounder;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class StoreFounder : StoreOwner
    {
        public StoreFounder(DomainStoreFounder domainFounder): base(domainFounder)
        {
        }
    }
}
