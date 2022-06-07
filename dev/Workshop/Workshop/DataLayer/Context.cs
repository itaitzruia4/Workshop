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

namespace Workshop.DataLayer
{
    public class Context: DbContext
    {
        public DbSet<MarketController> marketController { get; set; }
        public DbSet<UserController> userController { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Data Source = 34.107.89.228;Initial Catalog=WorkshopDB; Integrated Security = False; User Id = sqlserver; Password = workshop; Encrypt = True; TrustServerCertificate = True; MultipleActiveResultSets = True");
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            Logger.Instance.LogEvent($"Updating {entity} in the cache");
            return base.Update(entity);
        }

        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            Logger.Instance.LogEvent($"Adding {entity} to the cache");
            return base.Add(entity);
        }

    }
}
