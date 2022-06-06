using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Members;
<<<<<<< HEAD
using Action = Workshop.DataLayer.DataObjects.Members.Action;
using Workshop.DataLayer.DataObjects.Market.Discounts;
=======
using Workshop.DataLayer.DataObjects.Controllers;
>>>>>>> a435401cdcd8cb032971602a6846e124fbc1c81b

namespace Workshop.DataLayer
{
    public class Context: DbContext
    {
        DbSet<MarketController> marketController { get; set; }
        DbSet<UserController> userController { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Data Source = 34.107.89.228;Initial Catalog=WorkshopDB; Integrated Security = False; User Id = sqlserver; Password = workshop; Encrypt = True; TrustServerCertificate = True; MultipleActiveResultSets = True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscountPolicy>();
            modelBuilder.Entity<Product>();
            modelBuilder.Entity<PurchasePolicy>();
            modelBuilder.Entity<Review>();
            modelBuilder.Entity<ShoppingBag>();
            modelBuilder.Entity<ShoppingBagProduct>();
            modelBuilder.Entity<ShoppingCart>();
            modelBuilder.Entity<Store>();
            modelBuilder.Entity<Action>();
            modelBuilder.Entity<MarketManager>();
            modelBuilder.Entity<Member>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<StoreFounder>();
            modelBuilder.Entity<StoreManager>();
            modelBuilder.Entity<StoreOwner>();
            modelBuilder.Entity<StoreRole>();
        }

    }
}
