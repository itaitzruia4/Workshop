using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHandler = Workshop.DataLayer.DataHandler;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreManager: StoreRole
    {
        public StoreManager(int storeId) : base(storeId)
        {
            roleDAL.RoleType = "StoreManager";

            DataHandler.Instance.Value.save(roleDAL);
        }

        public StoreManager(RoleDAL roleDAL) : base(roleDAL)
        { }

    }
}
