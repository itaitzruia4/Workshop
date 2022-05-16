using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainStoreManager = Workshop.DomainLayer.UserPackage.Permissions.StoreManager;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class StoreManager : StoreRole
    {
        public StoreManager(DomainStoreManager domainManager) : base(domainManager)
        {
        }
    }
}
