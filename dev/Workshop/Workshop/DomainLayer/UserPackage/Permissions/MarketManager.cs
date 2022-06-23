using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALObject = Workshop.DataLayer.DALObject;
using MarketManagerDAL = Workshop.DataLayer.DataObjects.Members.Role;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using DataHandler = Workshop.DataLayer.DataHandler;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;


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
            actions.Add(Action.GetMembersOnlineStats);
            actions.Add(Action.ViewStorePurchaseHistory);

            roleDAL.RoleType = "MarketManager";
            foreach (var action in actions)
                roleDAL.Actions.Add(new ActionDAL((int)action));

            DataHandler.getDBHandler().save(roleDAL);
        }

        public MarketManager(RoleDAL roleDAL) : base(roleDAL)
        {}

        
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
