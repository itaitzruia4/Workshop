﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    class Member : User
    {
        private string username;
        private string password;

        public Member(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        internal string GetPassword()
        {
            return password;
        }

        internal string GetUsername()
        {
            return username;
        }
    }
}