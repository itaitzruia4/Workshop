using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Role : DALObject
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<Action> Actions { get; set; }
        public string RoleType { get; set; }
        public int StoreId { get; set; }
        public List<NameToRole> nominees { get; set; }

        public Role()
        {
            this.Id = nextId;
            nextId++;
        }

        // Consructor for non-store role
        public Role(int StoreId, List<Action> actions, string RoleType)
        {
            this.StoreId = StoreId;
            this.Actions = actions;
            this.RoleType = RoleType;
            Actions = new List<Action>();
            nominees = new List<NameToRole>();
            this.Id = nextId;
            nextId++;
        }

        // Consructor for non-store role
        public Role(int StoreId, List<Action> actions, string RoleType, List<NameToRole> nominees)
        {
            this.StoreId=StoreId;
            this.Actions = actions;
            this.RoleType = RoleType;
            this.StoreId = StoreId;
            this.nominees = nominees;
            Actions = new List<Action>();
            this.Id = nextId;
            nextId++;
        }
    }
}
