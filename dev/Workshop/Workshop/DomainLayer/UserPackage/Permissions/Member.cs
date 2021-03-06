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
            shoppingCart = new MemberShoppingCart();

            memberDAL = new MemberDAL(password, username, birthdate, new List<RoleDAL>(), ((MemberShoppingCart)shoppingCart).ToDAL());
            DataHandler.Instance.Value.save(memberDAL);
        }

        public Member(MemberDAL memberDAL)
        {
            this.memberDAL = memberDAL;
            Username = memberDAL.MemberName;
            Password = memberDAL.Password;
            Birthdate = memberDAL.Birthdate;
            roles = new List<Role>();
            foreach (RoleDAL role in memberDAL.Roles)
                roles.Add(Role.createRole(role));
            this.rwl = new ReaderWriterLock();
            shoppingCart = new MemberShoppingCart(memberDAL.ShoppingCart);
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
                if (role.GetType().Equals(role2.GetType()))
                {
                    rwl.ReleaseReaderLock();
                    throw new InvalidOperationException("This user is already holding the requested role.");
                }
            }
            LockCookie lc = rwl.UpgradeToWriterLock(Timeout.Infinite);
            roles.Add(role);
            memberDAL.Roles.Add(role.ToDAL());
            DataHandler.Instance.Value.update(memberDAL);
            rwl.DowngradeFromWriterLock(ref lc);
            rwl.ReleaseReaderLock();
        }

        public void RemoveRole(Role role)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);

            if(roles.Contains(role))
            {
                LockCookie lc = rwl.UpgradeToWriterLock(Timeout.Infinite);
                roles.Remove(role);
                memberDAL.Roles.Remove(role.ToDAL());
                DataHandler.Instance.Value.update(memberDAL);
                rwl.DowngradeFromWriterLock(ref lc);
                rwl.ReleaseReaderLock();
                return;
            }
                
            /*
            foreach (Role role2 in roles)
            {
                if (role.Equals(role2))
                {
                    LockCookie lc = rwl.UpgradeToWriterLock(Timeout.Infinite);
                    roles.Remove(role2); //TODO fix deleting from collection while iteration over it
                    memberDAL.Roles.Remove(role2.ToDAL());
                    DataHandler.Instance.Value.update(memberDAL);
                    rwl.DowngradeFromWriterLock(ref lc);
                    rwl.ReleaseReaderLock();
                    return;
                }
            }
            */
            
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
