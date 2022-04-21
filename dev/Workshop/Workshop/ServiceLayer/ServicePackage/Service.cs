using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer;
using Workshop.ServiceLayer.ServiceObjects;
using DomainUser = Workshop.DomainLayer.UserPackage.User;
using DomainMember = Workshop.DomainLayer.UserPackage.Permissions.Member;
using DomainProduct = Workshop.DomainLayer.MarketPackage.Product;
using DomainStoreManager = Workshop.DomainLayer.UserPackage.Permissions.StoreManager;
using DomainStoreOwner = Workshop.DomainLayer.UserPackage.Permissions.StoreOwner;
using DomainStoreFounder = Workshop.DomainLayer.UserPackage.Permissions.StoreFounder;

namespace Workshop.ServiceLayer
{
    class Service : IService
    {
        private Facade Facade;

        public Service(Facade facade)
        {
            Facade = facade;
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

        public Response<Product> AddProduct(string username, int storeId, int productId, string productName, string description, double price, int quantity)
        {
            try
            {
                DomainProduct domainProduct = Facade.AddProduct(username, storeId, productId, productName, description, price, quantity);
                Product serviceProduct = new Product(domainProduct);
                return new Response<Product>(serviceProduct);
            }
            catch (Exception e)
            {
                return new Response<Product>(e.Message);
            }
        }

        public Response<StoreOwner> NominateStoreOwner(string nominatorUsername, string nominatedUsername, int storeId)
        {
            try
            {
                DomainStoreOwner domainOwner = Facade.NominateStoreOwner(nominatorUsername, nominatedUsername, storeId);
                StoreOwner serviceOwner = new StoreOwner(domainOwner);
                return new Response<StoreOwner>(serviceOwner);
            }
            catch (Exception e)
            {
                return new Response<StoreOwner>(e.Message);
            }
        }

        public Response<StoreManager> NominateStoreManager(string nominatorUsername, string nominatedUsername, int storeId)
        {
            try
            {
                DomainStoreManager domainManager = Facade.NominateStoreManager(nominatorUsername, nominatedUsername, storeId);
                StoreManager serviceManager = new StoreManager(domainManager);
                return new Response<StoreManager>(serviceManager);
            }
            catch (Exception e)
            {
                return new Response<StoreManager>(e.Message);
            }
        }

        public Response<List<Member>> GetWorkersInformation(string username, int storeId)
        {
            try
            {
                List<DomainMember> members = Facade.GetWorkersInformation(username, storeId);
                List<Member> returnMembers = members.Select(x => new Member(x)).ToList();
                return new Response<List<Member>>(returnMembers);
            }
            catch (Exception e)
            {
                return new Response<List<Member>>(e.Message);
            }
        }
        public Response CloseStore(string username, int storeId) 
        {
            try
            {
                Facade.CloseStore(username, storeId);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
}
