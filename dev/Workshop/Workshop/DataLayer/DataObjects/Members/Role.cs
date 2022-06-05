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

        public Role()
        { }

        public Role(List<Action> actions)
        {
            Actions = actions;
        }
    }
}
