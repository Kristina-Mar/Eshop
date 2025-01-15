using Eshop.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Persistence
{
    public class OrdersContext : DbContext
    {
        private readonly string connectionString;

        public OrdersContext(string connectionString = "Data Source=../../data/localdb.db")
        {
            this.connectionString = connectionString;
            this.Database.Migrate();
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}