using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Action : DALObject
    {
        public int Id { get; set; }
        public Action(int Id)
        {
            this.Id = Id;
        }
    }
}
