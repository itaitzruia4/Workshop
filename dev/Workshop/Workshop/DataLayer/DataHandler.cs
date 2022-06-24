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

        public T find<T>(object key) where T : class, DALObject
        {
            if (Context.USE_DB)
                return (T)cache.Find(typeof(T), key);
            return null;
        }

        public MarketController SearchMarket(int key)
        {
            return cache.marketController.Where(s => s.Id == key)
                    .Include(mc => mc.userController.members)
                    .Include(mc => mc.userController.reviewHandler.productReviews)
                    .Include(mc => mc.userController.reviewHandler.userReviews)
                    .Include(mc => mc.userController.notificationHandler.Notifications)
                    .Include(mc => mc.userController.notificationHandler.observers)
                    .Include(mc => mc.userController.orderHandler.MemberToOrders)
                    .FirstOrDefault();
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
