using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class StoreOwner: StoreRole
    {
        public StoreOwner(int StoreId, List<Action> actions, List<StoreRole> nominees) : base(StoreId, actions, nominees)
        {
        }
    }
}
