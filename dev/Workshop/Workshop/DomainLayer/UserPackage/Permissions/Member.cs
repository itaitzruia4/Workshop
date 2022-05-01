﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class Member : User
    {
        public string Username { get; }
        internal string Password { get; }

        private List<Role> roles;

        public Member(string username, string password)
        {
            Username = username;
            Password = password;
            roles = new List<Role>();
        }

        public bool IsAuthorized(Action action)
        {
            foreach (Role role in roles)
            {
                if (role.IsAuthorized(action))
                    return true;
            }
            return false;
        }

        public bool IsAuthorized(int storeID, Action action)
        {
            foreach (Role role in roles) 
            {
                if(role.IsAuthorized(storeID, action))
                    return true;
            }
            return false;
        }

        public void AddRole(Role role)
        {
            foreach(Role role2 in roles)
            {
                if (role.Equals(role2))
                    throw new InvalidOperationException("This user is already holding the requested role.");
            }
            roles.Add(role);
        }

        public List<StoreRole> GetStoreRoles(int storeId)
        {
            List<StoreRole> result = new List<StoreRole>();

            foreach(Role role in roles)
            {
                if (role is StoreRole)
                    result.Add((StoreRole) role);
            }

            return result;
        }
    }
}
