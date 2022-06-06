using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Role : DALObject
    {
        public int Id { get; set; }
        public List<Action> Actions { get; set; }
        public string RoleType { get; set; }
        public int StoreId { get; set; }
        public List<Role> nominees { get; set; }

        public Role()
        { }

        // Consructor for non-store role
        public Role(int StoreId, List<Action> actions, string RoleType)
        {
            this.StoreId = StoreId;
            this.Actions = actions;
            this.RoleType = RoleType;
            Actions = new List<Action>();
        }

        // Consructor for non-store role
        public Role(int StoreId, List<Action> actions, string RoleType, List<Role> nominees)
        {
            this.StoreId=StoreId;
            this.Actions = actions;
            this.RoleType = RoleType;
            this.StoreId = StoreId;
            this.nominees = nominees;
            Actions = new List<Action>();
        }
    }
}
