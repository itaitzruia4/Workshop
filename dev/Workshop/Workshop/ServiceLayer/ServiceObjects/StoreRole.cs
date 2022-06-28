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
        public Dictionary<string, StoreRole> Nominees { get; set; }

        public StoreRole(DomainStoreRole dsr) : base(dsr)
        {
            StoreId = dsr.StoreId;
            Nominees = new Dictionary<string, StoreRole>();
            Dictionary<string, DomainStoreRole> domain_roles = dsr.GetAllNominees();
            foreach (string s in domain_roles.Keys)
            {
                Nominees.Add(s, new StoreRole(domain_roles[s]));
            }
        }
    }
}
