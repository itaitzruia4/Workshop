using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class StoreRole: Role
    {
        public int StoreId { get; set; }
        public List<StoreRole> nominees { get; set; }
    }
}
