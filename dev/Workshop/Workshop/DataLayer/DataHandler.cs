using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Workshop.DataLayer.DataObjects.Market;
using System.Threading;
using Workshop.DomainLayer.Loggers;
using Microsoft.EntityFrameworkCore;
using Workshop.DataLayer.DataObjects.Controllers;
using Workshop.DataLayer.DataObjects.Market.Discounts;
using Workshop.DataLayer.DataObjects.Market.Purchases;
using Workshop.DataLayer.DataObjects.Members;
using Workshop.DataLayer.DataObjects.Notifications;
using Workshop.DataLayer.DataObjects.Orders;
using Workshop.DataLayer.DataObjects.Reviews;

namespace Workshop.DataLayer
{
    public class DataHandler
    {
        private Context cache;
        private static DataHandler Instance = null;


        private DataHandler()
        {
            cache = new Context();
            //cache.ChangeTracker.AutoDetectChangesEnabled = false;
            //new Thread(() => upload(2)).Start();
        }

        public static DataHandler getDBHandler()
        {
            if (Instance == null)
                Instance = new DataHandler();
                
            return Instance;
        }


        public void save<T>(T toSave) where T : class, DALObject
        {
            if (Context.USE_DB)
            {
                cache.Add(toSave);
                cache.SaveChanges();
            }
        }

        public void update<T>(T toUpdate) where T : class, DALObject
        {
            if (Context.USE_DB)
            {
                cache.Update(toUpdate);
                cache.SaveChanges();
            }
        }

        public void remove<T>(T toRemove) where T : class, DALObject
        {
            if (Context.USE_DB)
            {
                cache.Remove(toRemove);
                cache.SaveChanges();
            }
        }

        public void remove<T>(List<T> toRemove) where T : class, DALObject
        {
            if (Context.USE_DB)
            {   
                foreach (T to in toRemove)
                    cache.Remove(to);
                cache.SaveChanges();
            }
        }

        public T find<T>(object key) where T : class, DALObject
        {
            if (Context.USE_DB)
                return (T)cache.Find(typeof(T), key);
            return null;
        }

        public MarketController loadMarket(int key)
        {
            //Add for each member instance it's roles

            MarketController market = cache.marketController.Where(s => s.Id == key)
                    .Include(mc => mc.userController.members).ThenInclude(m => m.ShoppingCart).ThenInclude(s => s.ShoppingBags).ThenInclude(sb => sb.Products)
                    .Include(mc => mc.userController.members).ThenInclude(m => m.Roles).ThenInclude(r => r.nominees).ThenInclude(n => n.father)
                    .Include(mc => mc.userController.reviewHandler.productReviews).ThenInclude(pr => pr.userToReviewDTOs).ThenInclude(utr => utr.Review)
                    .Include(mc => mc.userController.reviewHandler.userReviews).ThenInclude(ur => ur.productToReviewDTOs).ThenInclude(ptr => ptr.Review)
                    .Include(mc => mc.userController.notificationHandler.Notifications).ThenInclude(mn => mn.Notifications)
                    .Include(mc => mc.userController.notificationHandler.observers).ThenInclude(eo => eo.Event)
                    .Include(mc => mc.userController.notificationHandler.observers).ThenInclude(eo => eo.Observers).ThenInclude(m => m.ShoppingCart).ThenInclude(s => s.ShoppingBags).ThenInclude(sb => sb.Products)
                    .Include(mc => mc.userController.orderHandler.MemberToOrders).ThenInclude(mto => mto.orders).ThenInclude(o => o.address)
                    .Include(mc => mc.userController.orderHandler.MemberToOrders).ThenInclude(mto => mto.orders).ThenInclude(o => o.items)
                    .Include(mc => mc.stores)
                    .Include(mc => mc.stores).ThenInclude(s => s.DiscountPolicy)
                    .Include(mc => mc.stores).ThenInclude(s => s.DiscountPolicy).ThenInclude(dp => dp.products_discounts).ThenInclude(pd => pd.Discount)
                    .Include(mc => mc.stores).ThenInclude(s => s.DiscountPolicy).ThenInclude(dp => dp.category_discounts).ThenInclude(pd => pd.Discount)
                    .Include(mc => mc.stores).ThenInclude(s => s.DiscountPolicy).ThenInclude(dp => dp.store_discount)
                    .Include(mc => mc.stores).ThenInclude(s => s.PurchasePolicy)
                    .Include(mc => mc.stores).ThenInclude(s => s.PurchasePolicy).ThenInclude(pp => pp.products_terms).ThenInclude(pt => pt.Term)
                    .Include(mc => mc.stores).ThenInclude(s => s.PurchasePolicy).ThenInclude(pp => pp.category_terms).ThenInclude(pt => pt.Term)
                    .Include(mc => mc.stores).ThenInclude(s => s.PurchasePolicy).ThenInclude(pp => pp.user_terms)
                    .Include(mc => mc.stores).ThenInclude(s => s.PurchasePolicy).ThenInclude(pp => pp.store_terms)
                    .Include(mc => mc.stores).ThenInclude(s => s.Products)
                    .Include(mc => mc.stores).ThenInclude(s => s.Owners)
                    .FirstOrDefault();


            int nextId = 0;
            cache.marketController.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            MarketController.nextId = nextId;

            nextId = 0;
            cache.Discount.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Discount.nextId = nextId;

            nextId = 0;
            cache.DiscountPolicy.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            DiscountPolicy.nextId = nextId;

            nextId = 0;
            cache.PurchasePolicy.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            PurchasePolicy.nextId = nextId;

            nextId = 0;
            cache.Term.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Term.nextId = nextId;

            nextId = 0;
            cache.Product.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Product.nextId = nextId;

            nextId = 0;
            cache.ProductDTO.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ProductDTO.nextId = nextId;

            nextId = 0;
            cache.ShoppingBag.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ShoppingBag.nextId = nextId;

            nextId = 0;
            cache.ShoppingBagProduct.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ShoppingBagProduct.nextId = nextId;

            nextId = 0;
            cache.ShoppingCart.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ShoppingCart.nextId = nextId;

            nextId = 0;
            cache.SupplyAddress.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            SupplyAddress.nextId = nextId;

            nextId = 0;
            cache.Action.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            DataObjects.Members.Action.nextId = nextId;

            nextId = 0;
            cache.NameToRole.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            NameToRole.nextId = nextId;

            nextId = 0;
            cache.Role.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Role.nextId = nextId;

            nextId = 0;
            cache.Event.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Event.nextId = nextId;

            nextId = 0;
            cache.EventObservers.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            EventObservers.nextId = nextId;

            nextId = 0;
            cache.MemberNotifications.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            MemberNotifications.nextId = nextId;

            nextId = 0;
            cache.Notification.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Notification.nextId = nextId;

            nextId = 0;
            cache.NotificationHandler.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            NotificationHandler.nextId = nextId;

            nextId = 0;
            cache.MemberToOrdersI.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            MemberToOrders<int>.nextId = nextId;

            nextId = 0;
            cache.MemberToOrdersS.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            MemberToOrders<string>.nextId = nextId;

            nextId = 0;
            cache.OrderDTO.ToList().ForEach(item => nextId = Math.Max(nextId, item.id + 1));
            OrderDTO.nextId = nextId;

            nextId = 0;
            cache.OrderHandlerI.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            OrderHandler<int>.nextId = nextId;

            nextId = 0;
            cache.OrderHandlerS.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            OrderHandler<string>.nextId = nextId;

            nextId = 0;
            cache.ProductReviews.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ProductReviews.nextId = nextId;

            nextId = 0;
            cache.ProductToReviewDTO.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ProductToReviewDTO.nextId = nextId;

            nextId = 0;
            cache.Review.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            Review.nextId = nextId;

            nextId = 0;
            cache.ReviewDTO.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ReviewDTO.nextId = nextId;

            nextId = 0;
            cache.ReviewHandler.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            ReviewHandler.nextId = nextId;

            nextId = 0;
            cache.UserReviews.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            UserReviews.nextId = nextId;

            nextId = 0;
            cache.UserToReviewDTO.ToList().ForEach(item => nextId = Math.Max(nextId, item.Id + 1));
            UserToReviewDTO.nextId = nextId;

            return market;
        }

        public void clear()
        {
            if (Context.USE_DB)
            {
                cache.marketController.RemoveRange(cache.marketController);
                cache.userController.RemoveRange(cache.userController);
                cache.NotificationHandler.RemoveRange(cache.NotificationHandler);
                cache.Discount.RemoveRange(cache.Discount);
                cache.DiscountPolicy.RemoveRange(cache.DiscountPolicy);
                cache.PurchasePolicy.RemoveRange(cache.PurchasePolicy);
                cache.Term.RemoveRange(cache.Term);
                cache.Product.RemoveRange(cache.Product);
                cache.ProductDTO.RemoveRange(cache.ProductDTO);
                cache.ShoppingBag.RemoveRange(cache.ShoppingBag);
                cache.ShoppingBagProduct.RemoveRange(cache.ShoppingBagProduct);
                cache.ShoppingCart.RemoveRange(cache.ShoppingCart);
                cache.Store.RemoveRange(cache.Store);
                cache.SupplyAddress.RemoveRange(cache.SupplyAddress);
                cache.Action.RemoveRange(cache.Action);
                cache.Member.RemoveRange(cache.Member);
                cache.NameToRole.RemoveRange(cache.NameToRole);
                cache.Role.RemoveRange(cache.Role);
                cache.Event.RemoveRange(cache.Event);
                cache.EventObservers.RemoveRange(cache.EventObservers);
                cache.MemberNotifications.RemoveRange(cache.MemberNotifications);
                cache.Notification.RemoveRange(cache.Notification);

                cache.MemberToOrdersI.RemoveRange(cache.MemberToOrdersI);
                cache.MemberToOrdersS.RemoveRange(cache.MemberToOrdersS);
                cache.OrderDTO.RemoveRange(cache.OrderDTO);
                cache.OrderHandlerI.RemoveRange(cache.OrderHandlerI);
                cache.OrderHandlerS.RemoveRange(cache.OrderHandlerS);
                cache.ProductReviews.RemoveRange(cache.ProductReviews);
                cache.ProductToReviewDTO.RemoveRange(cache.ProductToReviewDTO);
                cache.Review.RemoveRange(cache.Review);
                cache.ReviewDTO.RemoveRange(cache.ReviewDTO);
                cache.ReviewHandler.RemoveRange(cache.ReviewHandler);
                cache.UserReviews.RemoveRange(cache.UserReviews);
                cache.UserToReviewDTO.RemoveRange(cache.UserToReviewDTO);

                cache.SaveChanges();
            }
        }

        private void upload(double timeOutInSeconds)
        {
            while (true)
            {
                Logger.Instance.LogEvent($"Saving in {timeOutInSeconds} secs to the DB");
                Thread.Sleep((int)timeOutInSeconds * 1000);
                Logger.Instance.LogEvent($"Trying to upload data to DB");
                try
                {
                    cache.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogEvent($"Exception!!!!!!!!!!!!!!!! " + ex.Message + ex.InnerException);
                }
                Logger.Instance.LogEvent($"Upload succeded");

            }
        }

    }
}
