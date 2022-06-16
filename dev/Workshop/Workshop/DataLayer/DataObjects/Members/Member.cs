using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;

namespace Workshop.DataLayer.DataObjects.Members
{
    public class Member : DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MemberName { get; set; }
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }
        public List<Role> Roles { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        public Member()
        { 

        }
        public Member(string password, string memberName, DateTime birthdate, List<Role> roles, ShoppingCart shoppingCart)
        {
            Password = password;
            MemberName = memberName;
            Birthdate = birthdate;
            Roles = roles;
            ShoppingCart = shoppingCart;
        }
    }
}
