using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.ServiceLayer.ServiceObjects;

namespace Workshop.ServiceLayer
{
    public interface IService
    {
        Response<User> EnterMarket();

        Response Register(string username, string password);

        Response<Member> Login(string username, string password);

        Response Logout(string username);
    }
}
