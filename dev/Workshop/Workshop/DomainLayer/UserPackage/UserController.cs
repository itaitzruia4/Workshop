﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Workshop.DomainLayer.UserPackage
{
    public enum Action
    {
        AddProduct,
        RemoveProduct,
        ChangeProductName,
        ChangeProductPrice,
        ChangeProductQuantity,
        NominateStoreOwner
    }

    public class UserController: IUserController
    {
        private ISecurityHandler securityHandler;
        private OrderHandler<User> orderHandler;
        private Dictionary<string, Member> members;
        private User currentUser;

        public UserController(ISecurityHandler securityHandler)
        {
            this.securityHandler = securityHandler;
            currentUser = null;

            members = new Dictionary<string, Member>();
            InitializeSystem();
        }

        // Being called only from MarketController
        public void NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            if (!members.ContainsKey(nominatorUsername))
                throw new ArgumentException("Username " + nominatorUsername + " does not exist.");
            if (!members.ContainsKey(nominatedUsername))
                throw new ArgumentException("Username " + nominatedUsername + " does not exist.");
            if (!members[nominatorUsername].IsAuthorized(storeId, Action.NominateStoreOwner))
                throw new MemberAccessException("User " + nominatorUsername + " is not allowed to nominate owners in this store.");
            members[nominatedUsername].AddRole(new StoreOwner(storeId));
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
            // TODO: add some pre-defined users (with at least one market manager) and encrypt their passwords
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
            if(!IsMember(username))
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
        private void AssertCurrentUser(string username)
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
    }
}
