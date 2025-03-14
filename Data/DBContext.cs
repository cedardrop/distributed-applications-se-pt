using Microsoft.EntityFrameworkCore;
using BeverageWarehouseAPI.Models;

namespace BeverageWarehouseAPI.Data
{
    public class BeverageWarehouseDbContext : DbContext
    {
        public BeverageWarehouseDbContext(DbContextOptions<BeverageWarehouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
