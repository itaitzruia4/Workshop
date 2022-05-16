using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using DomainRole = Workshop.DomainLayer.UserPackage.Permissions.Role;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public abstract class Role
    {
        public IReadOnlyCollection<Action> Actions { get; set; }

        public Role(DomainRole dr)
        {
            Actions = dr.GetAllActions();
        }
    }
}
