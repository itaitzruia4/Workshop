using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using DomainUser = Workshop.DomainLayer.UserPackage.User;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class User
    {
        ShoppingCartDTO Cart { get; set; }
        public User(DomainUser DomainUser)
        {
            Cart = DomainUser.ViewShoppingCart();
        }
    }
}
