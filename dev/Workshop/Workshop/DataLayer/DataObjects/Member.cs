﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects
{
    internal class Member
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string MemberName { get; set; }
        public DateTime Birthdate { get; set; }
        private List<Role> Roles { get; set; }


        public override string ToString()
        {
            string rolesStr = "";
            foreach (Role role in Roles)
                rolesStr += $"{role}";
            return $"{Id} {Password} {MemberName} {Birthdate} {rolesStr}";
        }
    }
}
