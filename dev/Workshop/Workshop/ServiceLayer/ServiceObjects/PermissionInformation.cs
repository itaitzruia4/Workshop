using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
namespace Workshop.ServiceLayer.ServiceObjects
{
    public class PermissionInformation
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public int StoreId { get; set; }
        public List<Action> Permissions { get; set; }

        public PermissionInformation(int userId, string membername, int storeId, List<Action> permissions)
        {
            UserId = userId;
            Membername = membername;
            StoreId = storeId;
            Permissions = permissions;
        }
    }
}
