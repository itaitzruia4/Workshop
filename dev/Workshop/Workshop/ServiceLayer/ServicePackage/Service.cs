using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer;
using Workshop.ServiceLayer.ServiceObjects;
using DomainUser = Workshop.DomainLayer.UserPackage.User;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;

namespace Workshop.ServiceLayer
{
    class Service : IService
    {
        private Facade Facade;

        public Service(Facade facade)
        {
            this.Facade = facade;
        }

        public Response<User> EnterMarket()
        {
            try
            {
                DomainUser domainUser = Facade.EnterMarket();
                User serviceUser = new User(domainUser);
                return new Response<User>(serviceUser);
            }
            catch(Exception e)
            {
                return new Response<User>(e.Message);
            }
        }

        public Response Register(string username, string password)
        {
            try
            {
                Facade.Register(username, password);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response<Member> Login(string username, string password)
        {
            try
            {
                DomainMember domainMember = Facade.Login(username, password);
                Member serviceMember = new Member(domainMember);
                return new Response<Member>(serviceMember);
            }
            catch (Exception e)
            {
                return new Response<Member>(e.Message);
            }
        }

        public Response Logout(string username)
        {
            try
            {
                Facade.Logout(username);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
}
