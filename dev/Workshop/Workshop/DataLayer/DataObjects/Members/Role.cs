using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Role
    {
        public int RoleId { get; set; }
        public List<Action> Actions { get; set; }

        /*public override string ToString()
        {
            return $"{Id} {RoleName} {Actions}";
        }*/
    }
}
