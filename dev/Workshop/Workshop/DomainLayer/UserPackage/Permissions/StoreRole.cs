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

    public class StoreRole: Role
    {
        public int StoreId { get; }
        private List<StoreRole> nominees;

        public StoreRole(int storeId): base()
        {
            this.StoreId = storeId;
            this.nominees = new List<StoreRole>();
        }

        public override StoreRoleDAL ToDAL()
        {
            List<ActionDAL> actionsDAL = new List<ActionDAL>();
            List<StoreRoleDAL> nomineesDAL = new List<StoreRoleDAL>();

            foreach (Action action in actions)
            {
                actionsDAL.Add(new ActionDAL(((int)action)));
            }
            foreach (StoreRole storeRole in nominees)
            {
                nomineesDAL.Add((StoreRoleDAL)storeRole.ToDAL());
            }

            return new StoreRoleDAL(StoreId, actionsDAL, "RoleType",nomineesDAL);
        }
        
        public IReadOnlyList<StoreRole> GetAllNominees()
        {
            return new List<StoreRole>(nominees);
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

        public void AddNominee(StoreRole nominee)
        {
            this.nominees.Add(nominee);
        }

        public void RemoveNominee(StoreRole nominee)
        {
            this.nominees.Remove(nominee);
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
