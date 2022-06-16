using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SystemAdminDTO = Workshop.ServiceLayer.ServiceObjects.SystemAdminDTO;

namespace Workshop.DomainLayer.UserPackage
{
    public class UserController : IUserController, ILoginChecker
    {
        private ISecurityHandler securityHandler;
        private IReviewHandler reviewHandler;
        private NotificationHandler notificationHandler;
        private OrderHandler<string> orderHandler;
        private ConcurrentDictionary<string, Member> members;
        private ConcurrentDictionary<int, User> currentUsers;
        public UserController(ISecurityHandler securityHandler, IReviewHandler reviewHandler, List<SystemAdminDTO> systemAdmins)
        {
            this.securityHandler = securityHandler;
            currentUsers = new ConcurrentDictionary<int, User>();
            this.reviewHandler = reviewHandler;
            members = new ConcurrentDictionary<string, Member>();
            this.orderHandler = new OrderHandler<string>();
            notificationHandler = new NotificationHandler(this);
            InitializeAdmins(systemAdmins);
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
        public User EnterMarket(int userId)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to enter the market");
            User user = new User();
            if (currentUsers.TryAdd(userId, user))
            {
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
                Logger.Instance.LogEvent($"User {userId} has successfuly registered user {username}");
            else
                throw new ArgumentException($"Username {username} already exists");

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
        public KeyValuePair<Member, List<Notification>> Login(int userId, string username, string password)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to login member {username}");
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
            Logger.Instance.LogEvent($"Successfuly logged in user {userId} as member {username}");
            List<Notification> userNotifications = notificationHandler.GetNotifications(member.Username);
            notificationHandler.RemoveNotifications(member.Username);
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

        public void AddStoreFounder(string username, int storeId)
        {
            Member member = GetMember(username);
            member.AddRole(new StoreFounder(storeId));
        }

        /*// Being called only from MarketController
        public StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            // Check that nominator is the logged in member
            AssertCurrentUser(userId, nominatorUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(nominatedUsername);

            Member nominator = members[nominatorUsername], nominated = members[nominatedUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!nominator.IsAuthorized(storeId, Action.NominateStoreOwner))
                throw new MemberAccessException($"Member {nominatorUsername} is not allowed to nominate owners in store #{storeId}.");

            if (nominatorUsername.Equals(nominatedUsername))
            {
                throw new InvalidOperationException($"Member {nominatorUsername} cannot nominate itself to be a store owner.");
            }

            // Check that nominator is not a store owner and that there is no circular nomination
            List<StoreRole> nominatedStoreRoles = nominated.GetStoreRoles(storeId), nominatorStoreRoles = nominator.GetStoreRoles(storeId);

            foreach (StoreRole nominatedStoreRole in nominatedStoreRoles)
            {
                if (nominatedStoreRole is StoreOwner)
                    throw new InvalidOperationException($"Member {nominatedUsername} is already a store owner of store #{storeId}");

                foreach (StoreRole nominatorStoreRole in nominatorStoreRoles)
                {
                    if (nominatedStoreRole.ContainsNominee(nominatorStoreRole))
                        throw new InvalidOperationException($"Member {nominatedUsername} was already nominated by {nominatorUsername} or one of its nominators");
                }
            }

            // Finally, add the new role
            StoreOwner newRole = new StoreOwner(storeId);
            nominated.AddRole(newRole);

            // Add the new manager to the nominator's nominees list
            StoreRole nominatorStoreOwner = nominatorStoreRoles.Last();
            nominatorStoreOwner.AddNominee(nominatedUsername, newRole);

            RegisterToEvent(nominated.Username, new Event("RemoveStoreOwnerNominationFrom" + nominatedUsername, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("SaleInStore" + storeId, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("OpenStore" + storeId, "", "MarketController"));
            RegisterToEvent(nominated.Username, new Event("CloseStore" + storeId, "", "MarketController"));
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} successfuly nominated member {nominatedUsername} as a store owner of store {storeId}");
            return newRole;
        }*/

        // Being called only from MarketController
        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} is trying to nominate {nominatedUsername} as a store manager of store {storeId}");
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

            // Check that nominator is not a store owner
            if (nominatedStoreRoles.Count > 0)
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
            Logger.Instance.LogEvent("User " + user + " is trying to review product " + productId);
            AssertCurrentUser(userId, user);
            List<OrderDTO> orders = orderHandler.GetOrders(user);
            bool purchasedProduct = false;
            foreach (OrderDTO order in orders)
            {
                if (order.ContainsProduct(productId))
                {
                    purchasedProduct = true;
                    break;
                }
            }
            if (!purchasedProduct)
            {
                Logger.Instance.LogEvent("User " + user + " FAILED to review product " + productId);
                throw new ArgumentException($"Username {user} did not purchase product {productId}");
            }
            Logger.Instance.LogEvent("User " + user + " successfuly reviewed product " + productId);
            return reviewHandler.AddReview(user, productId, review, rating);
        }


        public ShoppingBagProduct AddToCart(int userId, ShoppingBagProduct product, int storeId)
        {
            //ShoppingBagProduct 
            Logger.Instance.LogEvent("User " + userId + " is trying to add a product to his cart from store " + storeId);
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
            if (!members.TryRemove(canceledUsername, out canceled))
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

        bool IUserController.IsConnected(int userId)
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
            catch (Exception ex)
            {
                throw new ArgumentException("SANITY CHECK: GETMEMBERPERMISSIONS");
            }
            return member.GetAllRoles().Select(r => new ServiceLayer.ServiceObjects.PermissionInformation(userId, membername, (r is StoreRole ? ((StoreRole)r).StoreId : -1), r.GetAllActions())).ToList();
        }
    }
}
