using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Members;
using Action = Workshop.DataLayer.DataObjects.Members.Action;
using Workshop.DataLayer.DataObjects.Market.Discounts;
using Workshop.DataLayer.DataObjects.Controllers;
using Workshop.DataLayer.DataObjects.Market.Purchases;
using Workshop.DataLayer.DataObjects.Reviews;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Workshop.DomainLayer.Loggers;
using Workshop.DataLayer.DataObjects.Notifications;
using Workshop.DataLayer.DataObjects.Orders;

namespace Workshop.DataLayer
{
    public class Context: DbContext
    {
        public static bool USE_DB;
        public List<DbSet<DALObject>> DbSetList;
        public DbSet<MarketController> marketController { get; set; }
        public DbSet<UserController> userController { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<Discount> ProductDiscount { get; set; }
        public DbSet<Discount> CategoryDiscount { get; set; }
        public DbSet<DiscountPolicy> DiscountPolicy { get; set; }
        public DbSet<PurchasePolicy> PurchasePolicy { get; set; }
        public DbSet<Term> Term { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductDTO> ProductDTO { get; set; }
        public DbSet<ShoppingBag> ShoppingBag { get; set; }
        public DbSet<ShoppingBagProduct> ShoppingBagProduct { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<SupplyAddress> SupplyAddress { get; set; }
        public DbSet<Action> Action { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<NameToRole> NameToRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventObservers> EventObservers { get; set; }
        public DbSet<MemberNotifications> MemberNotifications { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationHandler> NotificationHandler { get; set; }
        public DbSet<MemberToOrders<int>> MemberToOrdersI { get; set; }
        public DbSet<MemberToOrders<string>> MemberToOrdersS { get; set; }
        public DbSet<OrderDTO> OrderDTO { get; set; }
        public DbSet<OrderHandler<int>> OrderHandlerI { get; set; }
        public DbSet<OrderHandler<string>> OrderHandlerS { get; set; }
        public DbSet<ProductReviews> ProductReviews { get; set; }
        public DbSet<ProductToReviewDTO> ProductToReviewDTO { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<ReviewDTO> ReviewDTO { get; set; }
        public DbSet<ReviewHandler> ReviewHandler { get; set; }
        public DbSet<UserReviews> UserReviews { get; set; }
        public DbSet<UserToReviewDTO> UserToReviewDTO { get; set; }
        public DbSet<EventObserversToMembers> EventObserversToMembers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (USE_DB)
            {
                optionsBuilder.UseSqlServer("Data Source = 34.107.89.228;Initial Catalog=WorkshopDB; Integrated Security = False; User Id = sqlserver; Password = workshop; Encrypt = True; TrustServerCertificate = True; MultipleActiveResultSets = True");
                optionsBuilder.EnableSensitiveDataLogging();
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NameToRole>()
                .HasOne(ntr => ntr.father)
                .WithMany(r => r.nominees);

            modelBuilder.Entity<EventObserversToMembers>()
                .HasOne(eotm => eotm.EventObserver)
                .WithMany(eo => eo.Observers);

            modelBuilder.Entity<EventObserversToMembers>()
                .HasOne(eotm => eotm.member)
                .WithMany(eo => eo.EventObservers);

            base.OnModelCreating(modelBuilder);
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            if (USE_DB)
            {
                Logger.Instance.LogEvent($"Updating {entity} in the cache");
                return base.Update(entity);
            }
            return null;
        }

        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            if (USE_DB)
            {
                Logger.Instance.LogEvent($"Adding {entity} to the cache");
                return base.Add(entity);
            }
            return null;
        }

    }
}
