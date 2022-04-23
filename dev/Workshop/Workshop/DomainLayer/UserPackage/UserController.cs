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

namespace Workshop.DomainLayer.UserPackage
{
    public class UserController : IUserController
    {
        private ISecurityHandler securityHandler;
        private IReviewHandler reviewHandler;

        // TODO should user key be member ID or username?
        private OrderHandler<string> orderHandler;
        private Dictionary<string, Member> members;
        private User currentUser;
        public UserController(ISecurityHandler securityHandler, IReviewHandler reviewHandler)
        {
            this.securityHandler = securityHandler;
            currentUser = null;
            this.reviewHandler = reviewHandler;
            members = new Dictionary<string, Member>();
        }

        // Being called only from MarketController
        public StoreOwner NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            // Check that nominator is the logged in member
            AssertCurrentUser(nominatorUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(nominatedUsername);

            Member nominator = members[nominatorUsername], nominated = members[nominatedUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!nominator.IsAuthorized(storeId, Action.NominateStoreOwner))
                throw new MemberAccessException($"User {nominatorUsername} is not allowed to nominate owners in store #{storeId}.");

            // Check that nominated is not a store owner and that there is no circular nomination
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
            return newRole;
        }

        // Being called only from MarketController
        public StoreManager NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            // Check that nominator is the logged in member
            AssertCurrentUser(nominatorUsername);

            // Check that the nominated member is indeed a member
            EnsureMemberExists(nominatedUsername);

            Member nominator = members[nominatorUsername], nominated = members[nominatedUsername];

            // Check that the nominator is authorized to nominate a store owner
            if (!nominator.IsAuthorized(storeId, Action.NominateStoreManager))
                throw new MemberAccessException($"User {nominatorUsername} is not allowed to nominate managers in store #{storeId}.");

            List<StoreRole> nominatedStoreRoles = nominated.GetStoreRoles(storeId), nominatorStoreRoles = nominator.GetStoreRoles(storeId);

            // Check that nominated is not a store owner
            if (nominatedStoreRoles.Count > 0)
                throw new InvalidOperationException($"User {nominatedUsername} is already a store owner/manager of store #{storeId}");

            // Finally, add the new role
            StoreManager newRole = new StoreManager(storeId);
            nominated.AddRole(newRole);
            return newRole;
        }

        private void EnsureMemberExists(string username)
        {
            if (!IsMember(username))
                throw new ArgumentException($"Username {username} does not exist");
        }

        public void AddPermissionToStoreManager(string ownerUsername, string managerUsername, int storeId, Action permission)
        {
            // Check that owner is the logged in member
            AssertCurrentUser(ownerUsername);

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
                    return;
                }
            }
            // If not found manager role in roles list throw an exception.
            throw new ArgumentException($"User {managerUsername} is not a store manager of store {storeId}");
        }

        public void RemovePermissionFromStoreManager(string ownerUsername, string managerUsername, int storeId, Action permission)
        {
            // Check that owner is the logged in member
            AssertCurrentUser(ownerUsername);

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
                    return;
                }
            }
            // If not found manager role in roles list throw an exception.
            throw new ArgumentException($"User {managerUsername} is not a store manager of store {storeId}");
        }

        public bool IsAuthorized(string username, int storeId, Action action)
        {
            if (!members.ContainsKey(username))
                throw new ArgumentException("Username " + username + " does not exist.");
            return members[username].IsAuthorized(storeId, action);
        }

        /// <summary>
        /// Load all members of the system
        /// </summary>
        public void InitializeSystem()
        {
            Member member1 = new Member("member1", securityHandler.Encrypt("pass1"));
            member1.AddRole(new StoreFounder(1));
            member1.AddRole(new StoreFounder(2));
            member1.AddRole(new StoreFounder(3));
            member1.AddRole(new StoreFounder(4));
            member1.AddRole(new StoreFounder(5));

            Member member2 = new Member("member2", securityHandler.Encrypt("pass2"));           
            member2.AddRole(new StoreOwner(2));

            Member member3 = new Member("member3", securityHandler.Encrypt("pass3"));        
            member3.AddRole(new StoreManager(3));
            member3.AddRole(new StoreOwner(2));

            Member member4 = new Member("member4", securityHandler.Encrypt("pass4"));
            member4.AddRole(new MarketManager());

            Member member5 = new Member("member5", securityHandler.Encrypt("pass5"));

            members.Add(member1.Username, member1);
            members.Add(member2.Username, member2);
            members.Add(member3.Username, member3);
            members.Add(member4.Username, member4);
            members.Add(member5.Username, member5);
        }

        /// <summary>
        /// Enter the market as a Visitor, updating the current user of the system
        /// </summary>
        /// <returns>
        /// A <c>User</c> instance representing the guest who entered the market
        /// </returns>
        public User EnterMarket()
        {
            if (currentUser != null)
                throw new InvalidOperationException("You have already entered the market");

            currentUser = new User();
            return currentUser;
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
        public Member Login(string username, string password)
        {
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket();

            if (!IsMember(username))
                throw new ArgumentException($"Username {username} does not exist");

            if (currentUser is Member)
                throw new InvalidOperationException("User is already logged in");

            Member member = members[username];

            string encryptedTruePassword = member.Password,
                   encryptedPasswordInput = securityHandler.Encrypt(password);

            if (!encryptedPasswordInput.Equals(encryptedTruePassword))
                throw new ArgumentException("Wrong password");

            // TODO figure out how to support multiple logged in users at once
            currentUser = member;

            return member;
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

        private void EnsureEnteredMarket()
        {
            if (currentUser == null)
                throw new InvalidOperationException("You must enter the market first before logging in");
        }

        /// <summary>
        /// Perform a logout attempt. If successfull, the current member returns to be a visitor.
        /// </summary>
        /// <param name="username">Username of the user that requests to log out</param>
        public void Logout(string username)
        {
            if (!IsMember(username))
                throw new ArgumentException($"Username {username} does not exist");
            AssertCurrentUser(username);

            currentUser = new User();
        }

        /// <summary>
        /// Attempt a registeration action. If successful, a new member is added to the system.
        /// </summary>
        /// <param name="username">Username to be registered</param>
        /// <param name="password">Password of the user that registers to the system</param>
        public void Register(string username, string password)
        {
            EnsureNonEmptyUserDetails(username, password);
            EnsureEnteredMarket();

            if (IsMember(username))
                throw new ArgumentException($"Username {username} already exists");

            string encryptedPassword = securityHandler.Encrypt(password);
            Member newMember = new Member(username, encryptedPassword);
            members.Add(username, newMember);
        }

        /// <summary>
        /// Assert that current user is the user that 
        /// </summary>
        /// <param name="username"></param>
        public void AssertCurrentUser(string username)
        {
            if ((!(currentUser is Member)) || !((Member)currentUser).Username.Equals(username))
                throw new ArgumentException($"Username {username} is not logged in");
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

        public void ReviewProduct(string user, int productId, string review)
        {
            AssertCurrentUser(user);
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
                throw new ArgumentException($"Username {user} did not purchase product {productId}");
            }
            reviewHandler.AddReview(user, productId, review);
        }
    }
}
