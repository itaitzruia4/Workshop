using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class StoreManager: StoreRole
    {
        public StoreManager(int storeId) : base(storeId)
        {
            roleDAL.RoleType = "StoreManager";

            DataHandler.getDBHandler().update(roleDAL);
        }
    }
}
