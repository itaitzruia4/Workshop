using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALObject = Workshop.DataLayer.DALObject;
using MarketManagerDAL = Workshop.DataLayer.DataObjects.Members.Role;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class MarketManager : Role
    {
        public MarketManager()
        {
            actions.Add(Action.ViewClosedStore);
            actions.Add(Action.CancelMember);
            actions.Add(Action.GetMarketStatistics);
            actions.Add(Action.GetMembersOnlineStats); 
        }

        
        public override MarketManagerDAL ToDAL()
        {
            return base.ToDAL();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType() != typeof(MarketManager);
        }
    }
}
