using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class Member : User
    {
        public string Username { get; }
        internal string Password { get; }
        public DateTime Birthdate { get; }

        private List<Role> roles;

        private ReaderWriterLock rwl;

        public Member(string username, string password, DateTime birthdate)
        {
            Username = username;
            Password = password;
            if (birthdate >= DateTime.Now)
                throw new ArgumentException($"Member can not be born before this very moment: {username}");
            Birthdate = birthdate;
            roles = new List<Role>();
            this.rwl = new ReaderWriterLock();
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
    }
}
