using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Action : DALObject
    {
        [Key]
        public int ActionType { get; set; }
        public Action(int ActionType)
        {
            this.ActionType = ActionType;
        }
    }
}
