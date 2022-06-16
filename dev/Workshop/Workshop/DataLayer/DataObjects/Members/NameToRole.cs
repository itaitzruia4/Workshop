using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class NameToRole
    {


        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Role role { get; set; }
        public string memberName { get; set; }
        public NameToRole()
        {
            this.Id = nextId;
            nextId++;
        }

        public NameToRole(Role role, string memberName)
        {
            this.role = role;
            this.memberName = memberName;
            this.Id = nextId;
            nextId++;
        }
    }
}
