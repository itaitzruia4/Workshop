using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using DALObject = Workshop.DataLayer.DALObject;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using StoreRoleDAL = Workshop.DataLayer.DataObjects.Members.Role;
using NameToRole = Workshop.DataLayer.DataObjects.Members.NameToRole;
using Workshop.DataLayer;

namespace Workshop.DomainLayer.UserPackage.Permissions
{

    public abstract class StoreRole : Role
    {
        public int StoreId { get; }
        public Dictionary<string, StoreRole> nominees { get; }

        public StoreRole(int storeId): base()
        {
            StoreId = storeId;
            nominees = new Dictionary<string, StoreRole>();
            roleDAL.StoreId = storeId;
        }

        public StoreRole(StoreRoleDAL storeRoleDAL) : base(storeRoleDAL)
        {
            StoreId = storeRoleDAL.StoreId;
            nominees = new Dictionary<string, StoreRole>();

            foreach(NameToRole ntr in storeRoleDAL.nominees)
            {
                //nominees[ntr.memberName] = (StoreRole)createRole(ntr.data);
                StoreRoleDAL temp = DataHandler.Instance.Value.find<StoreRoleDAL>(ntr.data_key);
                if (temp != null)
                {
                    nominees[ntr.memberName] = (StoreRole)createRole(temp);
                }
            }
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
            if (nominees.ContainsKey(membername))
            {
                nominees[membername] = nominee;
            }
            else
            {
                nominees.Add(membername, nominee);
            }
            NameToRole ntr = new NameToRole(nominee.ToDAL().Id, membername);
            DataHandler.Instance.Value.save(ntr);
            roleDAL.nominees.Add(ntr);
            DataHandler.Instance.Value.update(roleDAL);
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
                nominees.Remove(key_to_remove);
                NameToRole to_remove = null;
                foreach (NameToRole nameToRole in roleDAL.nominees)
                {
                    StoreRoleDAL role = DataHandler.Instance.Value.find<StoreRoleDAL>(nameToRole.data_key);
                    if (role != null && role.Equals(nominee.ToDAL()))
                        to_remove = nameToRole;
                }
                roleDAL.nominees.Remove(to_remove);
                DataHandler.Instance.Value.update(roleDAL);
                DataHandler.Instance.Value.remove(to_remove);
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
