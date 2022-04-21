using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;

namespace Workshop.DomainLayer.UserPackage
{
    enum Action
    {
        AddProduct,
        RemoveProduct,
        ChangeProductName,
        ChangeProductPrice,
        ChangeProductQuantity,
        NominateStoreOwner
    }

    class UserController : IUserController
    {
        private ISecurityHandler securityHandler;
        private Dictionary<string, Member> members;
        private Member loggedInUser = null;

        public UserController(ISecurityHandler securityHandler)
        {
            this.securityHandler = securityHandler;
            LoadAllMembers();
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

        public void LoadAllMembers()
        {
            // TODO: add some pre-defined users (with at least one market manager) and encrypt their passwords
            throw new NotImplementedException();
        }

        public User EnterMarket()
        {
            throw new NotImplementedException();
        }

        public Member Login(string username, string password)
        {
            EnsureNonEmptyUserDetails(username, password);
     
            if (!members.ContainsKey(username))
                throw new ArgumentException($"Username {username} does not exist");

            Member member = members[username];

            string encryptedTruePassword = member.GetPassword(),
                   encryptedPasswordInput = securityHandler.Encrypt(password);

            if (!encryptedPasswordInput.Equals(encryptedTruePassword))
                throw new ArgumentException("Wrong password");

            // TODO figure out how to support multiple logged in users at once
            loggedInUser = member;

            return member;
        }

        private void EnsureNonEmptyUserDetails(string username, string password)
        {
            if (username == null || password == null)
                throw new ArgumentNullException("Username or password cannot be null");
            if (username.Trim().Equals("") || password.Trim().Equals(""))
                throw new ArgumentException("Username or password cannot be empty");
        }

        public void Logout(string username)
        {
            if(!members.ContainsKey(username))
                throw new ArgumentException($"Username {username} does not exist");
            if(!loggedInUser.GetUsername().Equals(username))
                throw new ArgumentException($"Username {username} is not logged in");

            loggedInUser = null;
        }

        public void Register(string username, string password)
        {
            EnsureNonEmptyUserDetails(username, password);

            if (members.ContainsKey(username))
                throw new ArgumentException($"Username {username} already exists");

            string encryptedPassword = securityHandler.Encrypt(password);
            Member newMember = new Member(username, encryptedPassword);
            members.Add(username, newMember);
        }
    }
}
