using Customer.Data.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Customer.Data.Context
{
    public class CustomerApiDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqliteConn = new SqliteConnection(@"DataSource = Customer.db");
            optionsBuilder.UseSqlite(sqliteConn);
        }
        public DbSet<Customers> Customers { get; set; }
    }
}
