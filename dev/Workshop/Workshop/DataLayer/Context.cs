using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects
{
    public class Context: DbContext
    {
        DbSet<Member> Members { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Data Source = 34.107.89.228,1433; Integrated Security = False; User Id = sqlserver; Password = workshop; Encrypt = True; TrustServerCertificate = True; MultipleActiveResultSets = True");
        }

    }
}
