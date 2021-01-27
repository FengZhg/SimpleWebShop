using Microsoft.EntityFrameworkCore;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backEnd;
using System.Configuration;

namespace backEnd
{

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }


        public DbSet<KeyWordIndexLcx> KeyWordIndex { get; set; }

        public DbSet<OrderIndexLcx> OrderIndex { get; set; }

        public DbSet<OrderLcx> Order { get; set; }

        public DbSet<UserLcx> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }

}
