using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage;
using Workshop.DomainLayer.UserPackage.Permissions;

namespace Workshop.DomainLayer
{
    public class Facade
    {
        private IUserController UserController;
        private IMarketController MarketController;

        internal Facade(IUserController userController, IMarketController marketController)
        {
            UserController = userController;
            MarketController = marketController;
        }

        public User EnterMarket()
        {
            return UserController.EnterMarket();
        }

        public Member Login(string username, string password)
        {
            return UserController.Login(username, password);
        }

        public void Logout(string username)
        {
            UserController.Logout(username);
        }

        public void Register(string username, string password)
        {
            UserController.Register(username, password);
        }
    }
}