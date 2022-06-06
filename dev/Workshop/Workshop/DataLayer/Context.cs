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

namespace Workshop.DataLayer
{
    public class Context: DbContext
    {
        DbSet<Member> Members { get; set; }

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
