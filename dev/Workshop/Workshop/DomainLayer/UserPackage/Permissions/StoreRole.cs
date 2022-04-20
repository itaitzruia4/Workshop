using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.UserPackage.Permissions
{

    enum Action
    {
        AddProduct,
        RemoveProduct,
        ChangeProductName,
        ChangeProductPrice
    }

    class StoreRole: Role
    {
        protected Dictionary<int, KeyValuePair<Store, HashSet<Action>>> stores_to_actions;

        public StoreRole()
        {
            stores_to_actions = new Dictionary<int, KeyValuePair<Store, HashSet<Action>>>();
        }

        public void AddProduct(int storeID, int productID)
        {
            if (!stores_to_actions.ContainsKey(storeID))
                throw new ArgumentException("Store " + storeID + " is not owned or managed by this user.");
            Store store = stores_to_actions[storeID].Key;
            HashSet<Action> actions = stores_to_actions[storeID].Value;
            if (!actions.Contains(Action.AddProduct))
                throw new MemberAccessException("The user does not have permission for adding new products.");
            store.AddProduct(productID);
        }

        public void RemoveProduct(int storeID, int productID)
        {
            if (!stores_to_actions.ContainsKey(storeID))
                throw new ArgumentException("Store " + storeID + " is not owned or managed by this user.");
            Store store = stores_to_actions[storeID].Key;
            HashSet<Action> actions = stores_to_actions[storeID].Value;
            if (!actions.Contains(Action.RemoveProduct))
                throw new MemberAccessException("The user does not have permission for removing products.");
            store.RemoveProduct(productID);
        }
    }
}
