﻿using System;
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

namespace Workshop.DomainLayer.UserPackage
{
    public class UserController : IUserController
    {
        private ISecurityHandler securityHandler;
        private IReviewHandler reviewHandler;

        // TODO should user key be member ID or username?
        private OrderHandler<string> orderHandler;
        private ConcurrentDictionary<string, Member> members;
        private ConcurrentDictionary<int, User> currentUsers;
        public UserController(ISecurityHandler securityHandler, IReviewHandler reviewHandler)
        {
            this.securityHandler = securityHandler;
            currentUsers = new ConcurrentDictionary<int, User>();
            this.reviewHandler = reviewHandler;
            members = new ConcurrentDictionary<string, Member>();
            this.orderHandler = new OrderHandler<string>();
        }

        //*************************************************************************************************************
        // System Actions:
        //*************************************************************************************************************

        /// <summary>
        /// Load all members of the system
        /// </summary>
        public void InitializeSystem()
        {
            Logger.Instance.LogEvent("Starting initializing the system - User Controller");
            Member admin = new Member("admin", securityHandler.Encrypt("admin"));
            admin.AddRole(new MarketManager());
            Logger.Instance.LogEvent("Finished initializing the system - User Controller");
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
            if (currentUsers.ContainsKey(userId))
                throw new InvalidOperationException($"User {userId} has already entered the market");
            currentUsers[userId] = new User();
            Logger.Instance.LogEvent($"User {userId} has entered the market successfuly");
            return currentUsers[userId];
        }

        /// <summary>
        /// User exits the market
        /// </summary>
        public void ExitMarket(int userId)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to exit the market");
            if (currentUsers[userId] == null)
            {
                throw new ArgumentException($"User {userId} has not entered the market");
            }
            currentUsers[userId] = null;
            Logger.Instance.LogEvent($"User {userId} has exited the market successfuly");
        }

        /// <summary>
        /// Attempt a registeration action. If successful, a new member is added to the system.
        /// </summary>
        /// <param name="username">Username to be registered</param>
        /// <param name="password">Password of the user that registers to the system</param>
        public void Register(int userId, string username, string password)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to register user {username}");
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket(userId);

            if (IsMember(username))
                throw new ArgumentException($"Username {username} already exists");

            string encryptedPassword = securityHandler.Encrypt(password);
            Member newMember = new Member(username, encryptedPassword);
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
        public Member Login(int userId, string username, string password)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to login member {username}");
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket(userId);

            if (!IsMember(username))
                throw new ArgumentException($"Username {username} does not exist");

            if (currentUsers[userId] is Member)
                throw new InvalidOperationException($"User {userId} is already logged in");

            Member member = members[username];

            string encryptedTruePassword = member.Password,
                   encryptedPasswordInput = securityHandler.Encrypt(password);

            if (!encryptedPasswordInput.Equals(encryptedTruePassword))
                throw new ArgumentException($"User {userId} has entered wrong password for member {username}");

            currentUsers[userId] = member;
            Logger.Instance.LogEvent($"Successfuly logged in user {userId} as member {username}");
            return member;
        }

        /// <summary>
        /// Perform a logout attempt. If successfull, the current member returns to be a visitor.
        /// </summary>
        /// <param name="username">Username of the user that requests to log out</param>
        public void Logout(int userId, string username)
        {
            Logger.Instance.LogEvent($"User {userId} is trying to log out member {username}");
            if (currentUsers[userId] == null)
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

        // Being called only from MarketController
        public StoreOwner NominateStoreOwner(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatedUsername} is trying to nominate {nominatedUsername} as a store owner of store {storeId}");
            // Check that nominator is the logged in member
            AssertCurrentUser(userId, nominatorUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(nominatedUsername);

            Member nominator = members[nominatorUsername], nominated = members[nominatedUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!nominator.IsAuthorized(storeId, Action.NominateStoreOwner))
                throw new MemberAccessException($"User {nominatorUsername} is not allowed to nominate owners in store #{storeId}.");

            if (nominatorUsername.Equals(nominatedUsername))
            {
                throw new InvalidOperationException($"User {nominatorUsername} cannot nominate itself to be a Store Owner");
            }

            // Check that nominator is not a store owner and that there is no circular nomination
            List<StoreRole> nominatedStoreRoles = nominated.GetStoreRoles(storeId), nominatorStoreRoles = nominator.GetStoreRoles(storeId);

            foreach (StoreRole nominatedStoreRole in nominatedStoreRoles)
            {
                if (nominatedStoreRole is StoreOwner)
                    throw new InvalidOperationException($"User {nominatedUsername} is already a store owner of store #{storeId}");

                foreach (StoreRole nominatorStoreRole in nominatorStoreRoles)
                {
                    if (nominatedStoreRole.ContainsNominee(nominatorStoreRole))
                        throw new InvalidOperationException($"User {nominatedUsername} was already nominated by {nominatorUsername} or one of its nominators");
                }
            }

            // Finally, add the new role
            StoreOwner newRole = new StoreOwner(storeId);
            nominated.AddRole(newRole);

            // Add the new manager to the nominator's nominees list
            StoreRole nominatorStoreOwner = nominatorStoreRoles.Last();
            nominatorStoreOwner.AddNominee(newRole);
            Logger.Instance.LogEvent($"User {userId} with member {nominatorUsername} successfuly nominated member {nominatedUsername} as a store owner of store {storeId}");
            return newRole;
        }

        // Being called only from MarketController
        public StoreManager NominateStoreManager(int userId, string nominatorUsername, string nominatedUsername, int storeId)
        {
            Logger.Instance.LogEvent($"User {userId} with member {nominatedUsername} is trying to nominate {nominatedUsername} as a store manager of store {storeId}");
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
            nominatorStoreRole.AddNominee(newRole);
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
            if (currentUsers[userId] == null)
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

        public ReviewDTO ReviewProduct(int userId, string user, int productId, string review)
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
            return reviewHandler.AddReview(user, productId, review);
        }


        public ShoppingBagProduct addToCart(int userId, string username, ShoppingBagProduct product, int storeId)
        {
            //ShoppingBagProduct 
            Logger.Instance.LogEvent("User " + username + " is trying to add a product to his cart from store " + storeId);
            AssertCurrentUser(userId, username);
            return this.currentUsers[userId].addToCart(product,storeId);
        }

        public ShoppingCartDTO viewCart(int userId, string user)
        {
            Logger.Instance.LogEvent("User " + user + " is trying to view his cart");
            AssertCurrentUser(userId, user);
            return currentUsers[userId].viewShopingCart();
        }
        public ShoppingCartDTO editCart(int userId, string user, int productId, int newQuantity)
        {
            Logger.Instance.LogEvent("User " + user + " is trying to edit the quantity of " + productId + " in his cart");
            AssertCurrentUser(userId, user);
            if(newQuantity < 0)
            {
                Logger.Instance.LogEvent("User " + user + " failed to edit the quantity of " + productId + " in his cart");
                throw new ArgumentException($"Quantity {newQuantity} can not be a negtive number");
            }
            if(newQuantity == 0)
            {
                currentUsers[userId].deleteFromCart(productId);
            }
            else
            {
                currentUsers[userId].changeQuantityInCart(productId,newQuantity);
            }
            Logger.Instance.LogEvent("User " + user + " successfuly edited the quantity of " + productId + " in his cart");
            return currentUsers[userId].viewShopingCart();
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
            AssertCurrentUser(userId, username);
            orderHandler.addOrder(order, username);
            Logger.Instance.LogEvent($"User {userId} with member {username} added new order with ID {order.id}");
        }
    }
}
