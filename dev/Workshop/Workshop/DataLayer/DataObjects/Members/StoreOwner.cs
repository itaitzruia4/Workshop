using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class StoreOwner: StoreRole
    {
        public List<StoreRole> nominees { get; set; }
    }
}
