using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Action : DALObject
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ActionType { get; set; }

        public Action()
        {
            this.Id = nextId;
            nextId++;
        }
        public Action(int ActionType)
        {
            this.ActionType = ActionType;
            this.Id = nextId;
            nextId++;
        }
    }
}
