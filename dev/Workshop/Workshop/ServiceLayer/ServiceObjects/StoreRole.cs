using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using DomainStoreRole = Workshop.DomainLayer.UserPackage.Permissions.StoreRole;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class StoreRole : Role
    {
        public int StoreId { get; set; }
        public List<StoreRole> Nominees { get; set; }

        public StoreRole(DomainStoreRole dsr) : base(dsr)
        {
            StoreId = dsr.StoreId;
            Nominees = new List<StoreRole>();
            foreach (DomainStoreRole dstr in dsr.GetAllNominees())
            {
                Nominees.Add(new StoreRole(dstr));
            }
        }
    }
}
