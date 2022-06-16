using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemberDAL = Workshop.DataLayer.DataObjects.Members.Member;
using DALObject = Workshop.DataLayer.DALObject;
using RoleDAL = Workshop.DataLayer.DataObjects.Members.Role;
using ShoppingCartDAL = Workshop.DataLayer.DataObjects.Market.ShoppingCart;
using DataHandler = Workshop.DataLayer.DataHandler;
using Workshop.DomainLayer.UserPackage.Shopping;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class Member : User, IPersistentObject<MemberDAL>
    {
        public string Username { get; }
        internal string Password { get; }
        public DateTime Birthdate { get; }

        private List<Role> roles;

        private ReaderWriterLock rwl;

        private MemberDAL memberDAL;

        public Member(string username, string password, DateTime birthdate)
        {
            Username = username;
            Password = password;
            if (birthdate >= DateTime.Now)
                throw new ArgumentException($"Member can not be born before this very moment: {username}");
            Birthdate = birthdate;
            roles = new List<Role>();
            this.rwl = new ReaderWriterLock();
            shoppingCart = new MemberShoppingCart(shoppingCart);

            memberDAL = new MemberDAL(password, username, birthdate, new List<RoleDAL>(), ((MemberShoppingCart)shoppingCart).ToDAL());
            DataHandler.getDBHandler().save(memberDAL);
        }

        public MemberDAL ToDAL()
        {
            return memberDAL;
        }

        public IReadOnlyList<Role> GetAllRoles()
        {
            return new List<Role>(roles);
        }

        public bool IsAuthorized(Action action)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            foreach (Role role in roles)
            {
                if (role.IsAuthorized(action))
                {
                    rwl.ReleaseReaderLock();
                    return true;
                }
            }
            rwl.ReleaseReaderLock();
            return false;
        }

        public bool IsAuthorized(int storeID, Action action)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            foreach (Role role in roles) 
            {
                if (role.IsAuthorized(storeID, action))
                {
                    rwl.ReleaseReaderLock();
                    return true;
                }
            }
            rwl.ReleaseReaderLock();
            return false;
        }

        public void AddRole(Role role)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            foreach(Role role2 in roles)
            {
                if (role.Equals(role2))
                {
                    rwl.ReleaseReaderLock();
                    throw new InvalidOperationException("This user is already holding the requested role.");
                }
            }
            LockCookie lc = rwl.UpgradeToWriterLock(Timeout.Infinite);
            roles.Add(role);
            memberDAL.Roles.Add(role.ToDAL());
            DataHandler.getDBHandler().update(memberDAL);
            rwl.DowngradeFromWriterLock(ref lc);
            rwl.ReleaseReaderLock();
        }

        public void RemoveRole(Role role)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            foreach (Role role2 in roles)
            {
                if (role.Equals(role2))
                {
                    LockCookie lc = rwl.UpgradeToWriterLock(Timeout.Infinite);
                    roles.Remove(role2);
                    memberDAL.Roles.Remove(role2.ToDAL());
                    DataHandler.getDBHandler().update(memberDAL);
                    rwl.DowngradeFromWriterLock(ref lc);
                    rwl.ReleaseReaderLock();
                    return;
                }
            }
            
            rwl.ReleaseReaderLock();
            throw new InvalidOperationException($"Member {this.Username} does not have the requested role.");
        }

        public List<StoreRole> GetStoreRoles(int storeId)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            List<StoreRole> result = new List<StoreRole>();

            foreach(Role role in roles)
            {
                if (role is StoreRole)
                    result.Add((StoreRole) role);
            }

            rwl.ReleaseReaderLock();
            return result;
        }

        public bool HasRoles()
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            int count = roles.Count;
            rwl.ReleaseReaderLock();
            return count != 0;
        }

        public MemberDTO GetMemberDTO()
        {
            return new MemberDTO(Username, Password, roles);
        }
    }
}
