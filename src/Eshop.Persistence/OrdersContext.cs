using Eshop.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Persistence
{
    public class OrdersContext : DbContext
    {
        private readonly string connectionString;

        public OrdersContext()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Data Source=../../data/localdb.db";
            this.Database.Migrate();
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}