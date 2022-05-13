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

        private List<Role> roles;
        private ReaderWriterLock rwl;

        public Member(string username, string password)
        {
            Username = username;
            Password = password;
            roles = new List<Role>();
            this.rwl = new ReaderWriterLock();
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
