using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Members;
using Workshop.DataLayer.DataObjects.Controllers;

namespace Workshop.DataLayer.DataObjects
{
    public class Context: DbContext
    {
        DbSet<MarketController> marketController { get; set; }
        DbSet<UserController> userController { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Data Source = 34.107.89.228;Initial Catalog=WorkshopDB; Integrated Security = False; User Id = sqlserver; Password = workshop; Encrypt = True; TrustServerCertificate = True; MultipleActiveResultSets = True");
        }

    }
}
