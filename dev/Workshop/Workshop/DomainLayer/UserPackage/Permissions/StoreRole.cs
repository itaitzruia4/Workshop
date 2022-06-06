using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using DALObject = Workshop.DataLayer.DALObject;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using StoreRoleDAL = Workshop.DataLayer.DataObjects.Members.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{

    public class StoreRole : Role
    {
        public int StoreId { get; }
        public Dictionary<string, StoreRole> nominees { get; }

        public StoreRole(int storeId): base()
        {
            this.StoreId = storeId;
            this.nominees = new Dictionary<string, StoreRole>();
            this.roleDAL.StoreId = storeId;
        }

        public Dictionary<string, StoreRole> GetAllNominees()
        {
            return new Dictionary<string, StoreRole>(nominees);
        }

        public override bool IsAuthorized(int storeID, Action action)
        {
            if(this.StoreId != storeID)
                return false;

            return base.IsAuthorized(action);
        }

        public override bool Equals(object obj)
        {
            return obj is StoreRole && StoreId == ((StoreRole)obj).StoreId;
        }

        public void AddNominee(string membername, StoreRole nominee)
        {
            this.nominees.Add(membername, nominee);
        }

        public void RemoveNominee(StoreRole nominee)
        {
            string key_to_remove = null;
            foreach (string membername in this.nominees.Keys)
            {
                if (this.nominees[membername].Equals(nominee))
                {
                    key_to_remove = membername;
                    break;
                }
            }
            if (key_to_remove != null)
            {
                this.nominees.Remove(key_to_remove);
            }
        }

        /// <summary>
        /// Check for circularity in Store Role nominees
        /// </summary>
        /// <param name="candidate">The StoreRole candidate to be nominated </param>
        /// <returns>True if the candidate is already a nominee of this StoreRole, false otherwise</returns>
        public bool ContainsNominee(StoreRole candidate)
        {
            foreach(StoreRole nominee in this.nominees.Values)
            {
                if (nominee.Equals(candidate) || nominee.ContainsNominee(candidate))
                    return true;
            }
            return false;
        }
    }
}
