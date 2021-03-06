using System;
using System.Linq;
using System.Collections.Generic;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.Reviews;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.Loggers;
using System.Collections.Concurrent;
using Workshop.DomainLayer.UserPackage.Notifications;
using UserControllerDAL = Workshop.DataLayer.DataObjects.Controllers.UserController;
using MemberDAL = Workshop.DataLayer.DataObjects.Members.Member;
using DataHandler = Workshop.DataLayer.DataHandler;
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;
using System.Collections;

namespace Workshop.DomainLayer.UserPackage
{
    public class UserController : IUserController, ILoginChecker, IPersistentObject<UserControllerDAL>
    {
        private ISecurityHandler securityHandler;
        private IReviewHandler reviewHandler;
        private NotificationHandler notificationHandler;
        private OrderHandler<string> orderHandler;
        private ConcurrentDictionary<string, Member> members;
        private ConcurrentDictionary<int, User> currentUsers;

        public UserControllerDAL userControllerDAL { get; set; }
        //public UserController(ISecurityHandler securityHandler, IReviewHandler reviewHandler)
        public SortedList userCountOnDatePerType;
        public UserController(ISecurityHandler securityHandler, IReviewHandler reviewHandler, List<SystemAdminDTO> systemAdmins)
        {
            this.securityHandler = securityHandler;
            currentUsers = new ConcurrentDictionary<int, User>();
            this.reviewHandler = reviewHandler;
            members = new ConcurrentDictionary<string, Member>();
            this.orderHandler = new OrderHandler<string>();
            // TODO Find out how to actually implement IMessageSender, Look at Github Issues!
            notificationHandler = new NotificationHandler(this);
            userControllerDAL = new UserControllerDAL(reviewHandler.ToDAL(), notificationHandler.ToDAL(), orderHandler.ToDAL(), new List<MemberDAL>());
            DataHandler.Instance.Value.save(userControllerDAL);
            //InitializeSystem();
            userCountOnDatePerType = SortedList.Synchronized(new SortedList());
            InitializeAdmins(systemAdmins);
        }

        public UserController(UserControllerDAL userControllerDAL, List<SystemAdminDTO> systemAdmins)
        {
            this.userControllerDAL = userControllerDAL;
            this.securityHandler = new HashSecurityHandler();
            currentUsers = new ConcurrentDictionary<int, User>();
            this.reviewHandler = new ReviewHandler(userControllerDAL.reviewHandler);
            members = new ConcurrentDictionary<string, Member>();
            foreach (MemberDAL memberDAL in userControllerDAL.members)
                members[memberDAL.MemberName] = new Member(memberDAL);
            orderHandler = new OrderHandler<string>(userControllerDAL.orderHandler);
            notificationHandler = new NotificationHandler(userControllerDAL.notificationHandler, this);
            userCountOnDatePerType = SortedList.Synchronized(new SortedList());
        }

        public UserControllerDAL ToDAL()
        {
            return userControllerDAL;
        }

        private void InitializeAdmins(List<SystemAdminDTO> admins)
        {
            foreach (SystemAdminDTO admin in admins)
            {
                Logger.Instance.LogEvent($"Started adding market manager permissions to {admin.Membername}");
                Member member = new Member(admin.Membername, securityHandler.Encrypt(admin.Password), admin.Birthdate);
                member.AddRole(new MarketManager());
                members.TryAdd(member.Username, member);
                Logger.Instance.LogEvent($"Added market manager permissions to {admin.Membername}");
            }
        }

        public bool CheckOnlineStatus(string u)
        {

            foreach (User member in currentUsers.Values)
            {
                if (member is Member)
                {
                    if (((Member)member).Username == u)
                    {
                        return true;
                    }
                }

            }
            return false;
        }


        //*************************************************************************************************************
        // General Visitor-Guest Actions:
        //*************************************************************************************************************

        /// <summary>
        /// Enter the market as a Visitor, updating the current user of the system
        /// </summary>
        /// <returns>
        /// A <c>User</c> instance representing the guest who entered the market
        /// </returns>
        public User EnterMarket(int userId, DateTime date)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to enter the market");
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"{date} is not a valid date: you are not from the future!");
            }
            User user = new User();
            if (currentUsers.TryAdd(userId, user))
            {
                UpdateUserStatistics(user, date);
                Logger.Instance.LogEvent($"User {userId} has entered the market successfuly");
                return user;
            }
            throw new InvalidOperationException($"User {userId} has already entered the market");
        }

        /// <summary>
        /// User exits the market
        /// </summary>
        public void ExitMarket(int userId)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to exit the market");
            User user;
            if (!currentUsers.TryRemove(userId, out user))
            {
                throw new ArgumentException($"User {userId} has not entered the market");
            }
            Logger.Instance.LogEvent($"User {userId} has exited the market successfuly");
        }

        /// <summary>
        /// Attempt a registeration action. If successful, a new member is added to the system.
        /// </summary>
        /// <param name="username">Username to be registered</param>
        /// <param name="password">Password of the user that registers to the system</param>
        public void Register(int userId, string username, string password, DateTime birthdate)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to register user {username}");
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket(userId);

            if (IsMember(username))
                throw new ArgumentException($"Username {username} already exists");

            string encryptedPassword = securityHandler.Encrypt(password);
            Member newMember = new Member(username, encryptedPassword, birthdate);
            if (members.TryAdd(username, newMember))
            {
                //DataHandler.Instance.Value.update(userControllerDAL);
                Logger.Instance.LogEvent($"User {userId} has successfuly registered user {username}");
            }
            else
                throw new ArgumentException($"Username {username} already exists");
            userControllerDAL.members.Add(newMember.ToDAL());
            DataHandler.Instance.Value.update(userControllerDAL);
        }

        public void UpdateUserStatistics(User u, DateTime date)
        {
            lock (userCountOnDatePerType.SyncRoot)
            {
                if (userCountOnDatePerType.Contains(date.Date))
                {
                    ((UserCountInDate)userCountOnDatePerType[date.Date]).IncreaseCount(u);
                }
                else
                {
                    UserCountInDate userCount = new UserCountInDate(date.Date);
                    userCount.IncreaseCount(u);
                    userCountOnDatePerType.Add(date.Date, userCount);
                }
            }
        }

        /// <summary>
        /// Perform a login attempt. If successfull, the current visitor becomes a Member.
        /// </summary>
        /// <param name="username">
        /// Inserted username for login
        /// </param>
        /// <param name="password">
        /// Inserted password for login
        /// </param>
        /// <returns>
        /// The logged in member
        /// </returns>
        public KeyValuePair<Member, List<Notification>> Login(int userId, string username, string password, DateTime date)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to login member {username}");
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"{date} is not a valid date: you are not from the future!");
            }
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket(userId);

            if (currentUsers[userId] is Member)
                throw new InvalidOperationException($"User {userId} is already logged in");

            Member member;

            if (!members.TryGetValue(username, out member))
                throw new ArgumentException($"Username {username} does not exist");

            if (currentUsers.Values.Contains(member))
                throw new ArgumentException($"Member {username} is already logged in from another user");

            string encryptedTruePassword = member.Password,
               encryptedPasswordInput = securityHandler.Encrypt(password);

            if (!encryptedPasswordInput.Equals(encryptedTruePassword))
                throw new ArgumentException($"User {userId} has entered wrong password for member {username}");

            currentUsers[userId] = member;
            List<Notification> userNotifications = notificationHandler.GetNotifications(member.Username);
            notificationHandler.RemoveNotifications(member.Username);

            UpdateUserStatistics(member, date);

            Logger.Instance.LogEvent($"Successfuly logged in user {userId} as member {username}");
            return new KeyValuePair<Member, List<Notification>>(member, userNotifications);
        }

        /// <summary>
        /// Perform a logout attempt. If successfull, the current member returns to be a visitor.
        /// </summary>
        /// <param name="username">Username of the user that requests to log out</param>
        public void Logout(int userId, string username)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to log out member {username}");
            if (!currentUsers.ContainsKey(userId))
            {
                throw new ArgumentException($"User {userId} has not entered the market");
            }
            if (!(currentUsers[userId] is Member))
            {
                throw new ArgumentException($"User {userId} is not logged in as a member");
            }
            AssertCurrentUser(userId, username);

            currentUsers[userId] = new User();
            Logger.Instance.LogEvent($"Successfuly logged out user {userId} from member {username}");
        }

        /// <summary>
        /// Input verification for non-empty username and password
        /// </summary>
        /// <param name="username">
        /// Username inserted by the user
        /// </param>
        /// <param name="password">
        /// Password inserted by the user
        /// </param>
        private void EnsureNonEmptyUserDetails(string username, string password)
        {
            if (username == null || password == null)
                throw new ArgumentException("Username or password cannot be null");
            if (username.Trim().Equals("") || password.Trim().Equals(""))
                throw new ArgumentException("Username or password cannot be empty");
        }

        public void AddStoreFounder(string username, int storeId, DateTime date)
        {
            Member member = GetMember(username);
            member.AddRole(new StoreFounder(storeId));
            UpdateUserStatistics(member, date);
        }

        // Being called only from MarketController
        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId, DateTime date)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} is trying to nominate {nominatedUsername} as a store manager of store {storeId}");
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"{date} is not a valid date: you are not from the future!");
            }
            // Check that nominator is the logged in member
            AssertCurrentUser(userId, nominatorUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(nominatedUsername);

            Member nominator = members[nominatorUsername], nominated = members[nominatedUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!nominator.IsAuthorized(storeId, Action.NominateStoreManager))
                throw new MemberAccessException($"User {nominatorUsername} is not allowed to nominate managers in store #{storeId}.");

            if (nominatorUsername.Equals(nominatedUsername))
            {
                throw new InvalidOperationException($"User {nominatorUsername} cannot nominate itself to be a Store Manager");
            }

            List<StoreRole> nominatedStoreRoles = nominated.GetStoreRoles(storeId), nominatorStoreRoles = nominator.GetStoreRoles(storeId);

            // Check that nominee is not a store owner/manager already
            if (nominatedStoreRoles.Any(sr => sr is StoreManager))
                throw new InvalidOperationException($"User {nominatedUsername} is already a store owner/manager of store #{storeId}");

            // Finally, add the new role
            StoreManager newRole = new StoreManager(storeId);
            nominated.AddRole(newRole);

            // Add the new manager to the nominator's nominees list
            StoreRole nominatorStoreRole = nominatorStoreRoles.Last();
            nominatorStoreRole.AddNominee(nominatedUsername, newRole);

            RegisterToEvent(nominated.Username, new Event("RemoveStoreOwnerNominationFrom" + nominatedUsername, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("OpenStore" + storeId, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("CloseStore" + storeId, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("BidOfferInStore" + storeId, "", "MarketController"));
            UpdateUserStatistics(nominated, date);
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} successfuly nominated member {nominatedUsername} as a store manager of store {storeId}");
            return newRole;
        }

        private void EnsureMemberExists(string username)
        {
            if (!IsMember(username))
                throw new ArgumentException($"Username {username} does not exist");
        }

        public void AddPermissionToStoreManager(int userId, string ownerUsername, string managerUsername, int storeId, Action permission)
        {
            Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} is trying to add a permission to {managerUsername} in store {storeId}");
            // Check that owner is the logged in member
            AssertCurrentUser(userId, ownerUsername);

            // Check that the manager is indeed a member
            EnsureMemberExists(managerUsername);

            // Check that the owner is authorized for adding permissions
            IsAuthorized(ownerUsername, storeId, Action.AddPermissionToStoreManager);

            List<StoreRole> storeRoles = members[managerUsername].GetStoreRoles(storeId);

            // Find the manager StoreRole object
            foreach (StoreRole storeRole in storeRoles)
            {
                if (storeRole is StoreManager)
                {
                    // Add the permission to the manager
                    storeRole.AddAction(permission);
                    Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} successfuly to add a permission to {managerUsername} in store {storeId}");
                    return;
                }
            }
            // If not found manager role in roles list throw an exception.
            Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} FAILED to add a permission to {managerUsername} in store {storeId}");
            throw new ArgumentException($"Member {managerUsername} is not a store manager of store {storeId}");
        }

        public void RemovePermissionFromStoreManager(int userId, string ownerUsername, string managerUsername, int storeId, Action permission)
        {
            Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} is trying to remove a permission from {managerUsername} in store {storeId}");
            // Check that owner is the logged in member
            AssertCurrentUser(userId, ownerUsername);

            // Check that the manager is indeed a member
            EnsureMemberExists(managerUsername);

            // Check that the owner is authorized for adding permissions
            IsAuthorized(ownerUsername, storeId, Action.RemovePermissionFromStoreManager);

            List<StoreRole> storeRoles = members[managerUsername].GetStoreRoles(storeId);

            // Find the manager StoreRole object
            foreach (StoreRole storeRole in storeRoles)
            {
                if (storeRole is StoreManager)
                {
                    // Add the permission to the manager
                    storeRole.RemoveAction(permission);
                    Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} successfuly removed a permission from {managerUsername} in store {storeId}");
                    return;
                }
            }
            // If not found manager role in roles list throw an exception.
            Logger.Instance.LogEvent($"User {userId} with member {ownerUsername} FAILED to remove a permission from {managerUsername} in store {storeId}");
            throw new ArgumentException($"User {managerUsername} is not a store manager of store {storeId}");
        }

        public bool IsAuthorized(string username, int storeId, Action action)
        {
            if (!members.ContainsKey(username))
                throw new ArgumentException("Username " + username + " does not exist.");
            return members[username].IsAuthorized(storeId, action);
        }

        private void EnsureEnteredMarket(int userId)
        {
            if (!currentUsers.ContainsKey(userId))
                throw new InvalidOperationException($"User {userId} must enter the market first");
        }


        /// <summary>
        /// Assert that current user is the user that 
        /// </summary>
        /// <param name="username"></param>
        public void AssertCurrentUser(int userId, string username)
        {
            if (currentUsers[userId] == null)
            {
                throw new ArgumentException($"User {userId} has not entered the market");
            }
            if (!(currentUsers[userId] is Member))
            {
                throw new ArgumentException($"User {userId} is not logged in as a member");
            }
            if (!((Member)currentUsers[userId]).Username.Equals(username))
            {
                throw new ArgumentException($"User {userId} is not logged in as member {username}");
            }
        }

        /// <summary>
        /// Checks if a given username represents a registered member
        /// </summary>
        /// <param name="username">The name of the user to check</param>
        /// <returns>true if the username is a valid member, otherwise false</returns>
        public bool IsMember(string username)
        {
            return members.ContainsKey(username);
        }

        public List<Member> GetWorkers(int storeId)
        {
            List<Member> workers = new List<Member>();
            foreach (Member member in members.Values)
            {
                List<StoreRole> storeRoles = member.GetStoreRoles(storeId);
                if (storeRoles.Count != 0)
                    workers.Add(member);
            }
            return workers;
        }

        public Member GetMember(string username)
        {
            if (IsMember(username))
            {
                return members[username];
            }
            throw new ArgumentException($"Username {username} is not a member");
        }

        public int GetAge(int userId)
        {
            User user = null;
            AssertUserEnteredMarket(userId);
            if (currentUsers.TryGetValue(userId, out user))
            {
                if (user is Member)
                {
                    return (int)(DateTime.Now.Subtract(((Member)user).Birthdate).TotalDays / 365);
                }
                return -1;
            }
            return -1;
        }

        public ReviewDTO ReviewProduct(int userId, string user, int productId, string review, int rating)
        {
            Logger.Instance.LogEvent("User " + user + " is trying to review Product " + productId);
            AssertCurrentUser(userId, user);
            List<OrderDTO> orders = orderHandler.GetOrders(user);
            bool purchasedProduct = false;
            int storeId = -1;
            foreach (OrderDTO order in orders)
            {
                if (order.ContainsProduct(productId))
                {
                    purchasedProduct = true;
                    storeId = order.storeId;
                    break;
                }
            }
            if (!purchasedProduct)
            {
                Logger.Instance.LogEvent("User " + user + " FAILED to review Product " + productId);
                throw new ArgumentException($"Username {user} did not purchase Product {productId}");
            }
            Logger.Instance.LogEvent("User " + user + " successfuly reviewed Product " + productId);
            notify(new Event("ReviewInStore" + storeId, $"Member {user} has reviewed product {productId} in store {storeId}", "UserController"));
            return reviewHandler.AddReview(user, productId, review, rating);
        }


        public ShoppingBagProduct AddToCart(int userId, ShoppingBagProduct product, int storeId)
        {
            //ShoppingBagProduct 
            Logger.Instance.LogEvent("User " + userId + " is trying to add a Product to his cart from store " + storeId);
            return this.currentUsers[userId].AddToCart(product, storeId);
        }

        public ShoppingCartDTO viewCart(int userId)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to view his cart");
            AssertUserEnteredMarket(userId);
            return currentUsers[userId].ViewShoppingCart();
        }

        public void AssertUserEnteredMarket(int userId)
        {
            if (!currentUsers.ContainsKey(userId))
            {
                throw new ArgumentException($"User {userId} has not entered market");
            }
        }

        public void ClearUserCart(int userId)
        {
            if (!currentUsers.ContainsKey(userId))
            {
                throw new ArgumentException($"User {userId} does not exist");
            }
            currentUsers[userId].ClearCart();
        }

        public void AddOrder(int userId, OrderDTO order, string username)
        {
            Logger.Instance.LogEvent($"User {userId} with member {username} is trying to add new order with ID {order.id}");
            orderHandler.addOrder(order, username);
            Logger.Instance.LogEvent($"User {userId} with member {username} added new order with ID {order.id}");
        }

        public void CancelMember(int userId, string actingUsername, string canceledUsername)
        {
            Logger.Instance.LogEvent($"User {userId} with member {actingUsername} is trying to cancel member {canceledUsername}");
            // Check that nominator is the logged in member
            AssertCurrentUser(userId, actingUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(canceledUsername);

            Member actor = members[actingUsername], canceled = members[canceledUsername];

            // Check that the canceled member has a role
            if (canceled.HasRoles())
                throw new MemberAccessException($"User {canceledUsername} has roles so he cannot be canceled.");

            // Check that the nominator is authorized to do it
            if (!actor.IsAuthorized(Action.CancelMember))
                throw new MemberAccessException($"User {actingUsername} is not allowed to cancel members.");

            //cancel member
            if(members.TryRemove(canceledUsername,out canceled))
            {
                userControllerDAL.members.Remove(canceled.ToDAL());
                DataHandler.Instance.Value.save(userControllerDAL);
            }
            else
            {
                throw new ArgumentException($"Could not cancel member {canceledUsername}");
            }
            Logger.Instance.LogEvent($"User {userId} with member {actingUsername} successfuly canceled member {canceledUsername}");
        }

        public Dictionary<Member, bool> GetMembersOnlineStats(int userId, string actingUsername)
        {
            Logger.Instance.LogEvent($"User {userId} with member {actingUsername} is trying to get members online stats");
            // Check that nominator is the logged in member
            AssertCurrentUser(userId, actingUsername);

            Member actor = members[actingUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!actor.IsAuthorized(Action.GetMembersOnlineStats))
                throw new MemberAccessException($"User {actingUsername} is not allowed to get members online stats.");

            //get online members stats
            Dictionary<Member, bool> OnlineStats = new Dictionary<Member, bool>();
            foreach (Member member in members.Values)
            {
                OnlineStats.Add(member, CheckOnlineStatus(member.Username));
            }
            return OnlineStats;
        }

        public double GetProductRating(int productId)
        {
            return reviewHandler.GetProductRating(productId);
        }

        public bool IsConnected(int userId)
        {
            return currentUsers.ContainsKey(userId);
        }

        public void RegisterToEvent(string user, Notifications.Event @event)
        {
            notificationHandler.Attach(user, @event);
        }
        public void RemoveRegisterToEvent(string member, Notifications.Event @event)
        {
            notificationHandler.Detach(member, @event);
        }
        public void notify(Event @event)
        {
            notificationHandler.TriggerEvent(@event);
        }

        public User GetUser(int userId)
        {
            User u = null;
            if (currentUsers.TryGetValue(userId, out u))
            {
                return u;
            }
            throw new ArgumentException($"User {userId} has not entered market");
        }

        public List<Notification> TakeNotifications(int userId, string membername)
        {
            AssertCurrentUser(userId, membername);
            List<Notification> retme = notificationHandler.GetNotifications(membername);
            notificationHandler.RemoveNotifications(membername);
            return retme;
        }

        

        public List<ServiceLayer.ServiceObjects.PermissionInformation> GetMemberPermissions(int userId, string membername)
        {
            Func<Role, ServiceLayer.ServiceObjects.PermissionInformation> handleRole = (Role r) =>
            {
                return new ServiceLayer.ServiceObjects.PermissionInformation(userId, membername, (r is StoreRole ? ((StoreRole)r).StoreId : -1), r.GetAllActions());
            };
            AssertCurrentUser(userId, membername);
            Member member = null;
            try
            {
                member = (Member)GetUser(userId);
            }
            catch
            {
                throw new ArgumentException("SANITY CHECK: GETMEMBERPERMISSIONS");
            }
            return member.GetAllRoles().Select(r => new ServiceLayer.ServiceObjects.PermissionInformation(userId, membername, (r is StoreRole ? ((StoreRole)r).StoreId : -1), r.GetAllActions())).ToList();
        }

        private int bisect_left(DateTime[] l, DateTime val)
        {
            int low = 0, high = l.Length;
            while (low < high)
            {
                int mid = low + (high - low) / 2;
                if (l[mid] < val)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            return low;
        }

        private int bisect_right(DateTime[] l, DateTime val)
        {
            int low = 0, high = l.Length;
            while (low < high)
            {
                int mid = low + (high - low) / 2;
                if (l[mid] > val)
                {
                    high = mid;
                }
                else
                {
                    low = mid + 1;
                }
            }
            return low;
        }

        public List<UserCountInDate> MarketManagerDailyRangeInformation(int userId, string membername, DateTime beginning, DateTime end)
        {
            beginning = beginning.Date;
            end = end.Date;
            AssertCurrentUser(userId, membername);
            Member m = GetMember(membername);
            List<UserCountInDate> returnVal = new List<UserCountInDate>();
            if (!m.GetAllRoles().Any(x => x is MarketManager))
            {
                throw new ArgumentException($"{membername} is not a market manager and can not request to view this information.");
            }
            if (beginning == null || end == null || beginning > DateTime.Now || end > DateTime.Now || beginning > end)
            {
                throw new ArgumentException($"Given dates are not correct: {beginning}, {end}");
            }
            lock (userCountOnDatePerType.SyncRoot)
            {
                DateTime[] dates = userCountOnDatePerType.Keys.Cast<DateTime>().ToArray();
                int STARTING_INDEX = bisect_left(dates, beginning);
                int ENDING_INDEX = bisect_right(dates, end);
                for (int i = STARTING_INDEX; i < ENDING_INDEX; i++)
                {
                    returnVal.Add((UserCountInDate)userCountOnDatePerType.GetByIndex(i));
                }
            }
            return returnVal;
        }

        UserCountInDate IUserController.TodaysInformation(DateTime date)
        {
            lock (userCountOnDatePerType.SyncRoot)
            {
                if (userCountOnDatePerType.Contains(date.Date))
                {
                    return (UserCountInDate)userCountOnDatePerType[date.Date];
                }
                throw new ArgumentException("Could not find given date");
            }
        }
    }
}
