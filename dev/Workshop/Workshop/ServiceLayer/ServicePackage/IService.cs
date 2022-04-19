using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer.ServiceObjects;

namespace Workshop.ServiceLayer
{
    interface IService
    {
        void InitializeSystem();
        User EnterMarket();
        void Register(string username, string password);
        Member Login(string username, string password);
        void Logout(string username);
    }
}
