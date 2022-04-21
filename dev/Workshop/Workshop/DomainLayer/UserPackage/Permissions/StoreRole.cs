using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage.Permissions
{

    class StoreRole: Role
    {
        protected int storeId;
        private List<StoreRole> nominees;

        public StoreRole(int storeId): base()
        {
            this.storeId = storeId;
            this.nominees = new List<StoreRole>();
        }

        public override bool IsAuthorized(int storeID, Action action)
        {
            if(this.storeId != storeID)
                return false;
            return actions.Contains(action);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;
            if(obj.GetType() != typeof(StoreRole))
                return false;
            return storeId == ((StoreRole)obj).storeId;
        }

        public bool ExistInNomineesChain(StoreRole role)
        {

        }
    }
}
