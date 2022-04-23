using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;


namespace Workshop.ServiceLayer.ServiceObjects
{
    public abstract class Role
    {
        public IReadOnlyCollection<Action> actions;
    }
}
