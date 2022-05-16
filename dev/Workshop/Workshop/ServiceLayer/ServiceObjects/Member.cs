using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;
using DomainRole = Workshop.DomainLayer.UserPackage.Permissions.Role;
using DomainStoreOwner = Workshop.DomainLayer.UserPackage.Permissions.StoreOwner;
using DomainStoreManager = Workshop.DomainLayer.UserPackage.Permissions.StoreManager;
using DomainStoreFounder = Workshop.DomainLayer.UserPackage.Permissions.StoreFounder;
using DomainStoreRole = Workshop.DomainLayer.UserPackage.Permissions.StoreRole;
using DomainMarketManager = Workshop.DomainLayer.UserPackage.Permissions.MarketManager;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Member : User
    {
        public string Username { get; set; }
        public DateTime Birthdate { get; set; }
        private List<Role> Roles { get; set; }

        public Member(DomainMember domainMember): base(domainMember)
        {
            Username = domainMember.Username;
            Birthdate = domainMember.Birthdate;
            Roles = new List<Role>();
            foreach (DomainRole dr in domainMember.GetAllRoles())
            {
                if (dr is DomainStoreOwner)
                    Roles.Add(new StoreOwner((DomainStoreOwner)dr));
                if (dr is DomainStoreManager)
                    Roles.Add(new StoreManager((DomainStoreManager)dr));
                if (dr is DomainStoreFounder)
                    Roles.Add(new StoreFounder((DomainStoreFounder)dr));
                if (dr is DomainStoreRole)
                    Roles.Add(new StoreRole((DomainStoreRole)dr));
                if (dr is DomainMarketManager)
                    Roles.Add(new MarketManager((DomainMarketManager)dr));
            }
        }
    }
}
