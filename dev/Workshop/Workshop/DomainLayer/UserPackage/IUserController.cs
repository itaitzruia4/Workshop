using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Shopping;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using Notification = Workshop.DomainLayer.UserPackage.Notifications.Notification;
namespace Workshop.DomainLayer.UserPackage
{
    public interface IUserController
    {
        void AssertUserEnteredMarket(int userId);
        bool IsConnected(int userId);
        User EnterMarket(int userId, DateTime date);
        void ExitMarket(int userId);
        void UpdateUserStatistics(User u, DateTime date);
        void Register(int userId, string username, string password, DateTime birthdate);
        bool IsMember(string username);
        Member GetMember(string username);
        int GetAge(int userId);
        KeyValuePair<Member, List<Notification>> Login(int userId, string username, string password, DateTime date);
        void Logout(int userId, string username);
        //StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId);
        StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date);
        bool IsAuthorized(string username, int storeId, Action action);
        void AssertCurrentUser(int userId, string username);
        List<Member> GetWorkers(int storeId);
        ReviewDTO ReviewProduct(int userId, string user, int productId, string review, int rating);
        ShoppingBagProduct AddToCart(int userId, ShoppingBagProduct shoppingBagProduct, int storeId);
        ShoppingCartDTO viewCart(int userId);
        void AddStoreFounder(string username, int storeId, DateTime date);
        void AddOrder(int userId, OrderDTO order, string username);
        void ClearUserCart(int userId);
        double GetProductRating(int productId);
        void RegisterToEvent(string user, Notifications.Event @event);
        void RemoveRegisterToEvent(string MemberName, Notifications.Event @event);
        void notify(Notifications.Event @event);
        User GetUser(int userId);
        List<Notification> TakeNotifications(int userId, string membername);
        Dictionary<Member, bool> GetMembersOnlineStats(int userId, string actingUsername);
        void CancelMember(int userId, string actingUsername, string canceledUsername);
        List<ServiceLayer.ServiceObjects.PermissionInformation> GetMemberPermissions(int userId, string membername);
        Dictionary<string, Dictionary<string, dynamic>> MarketManagerDailyRangeInformation(int userId, string membername, DateTime beginning, DateTime end);
    }
}
