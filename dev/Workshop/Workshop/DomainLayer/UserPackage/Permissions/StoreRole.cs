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

            return base.IsAuthorized(action);
        }

        public override bool Equals(object obj)
        {
            return obj is StoreRole && storeId == ((StoreRole)obj).storeId;
        }

        /// <summary>
        /// Check for circularity in Store Role nominees
        /// </summary>
        /// <param name="candidate">The StoreRole candidate to be nominated </param>
        /// <returns>True if the candidate is already a nominee of this StoreRole, false otherwise</returns>
        public bool ContainsNominee(StoreRole candidate)
        {
            foreach(StoreRole nominee in this.nominees)
            {
                if (nominee.Equals(candidate) || nominee.ContainsNominee(candidate))
                    return true;
            }
            return false;
        }
    }
}
