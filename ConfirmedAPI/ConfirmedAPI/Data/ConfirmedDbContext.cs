using ConfirmedAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfirmedAPI.Data
{
    public class ConfirmedDbContext : DbContext
    {
        public ConfirmedDbContext(DbContextOptions<ConfirmedDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(r => r.ID);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Stock)
                .WithOne(s => s.Product)
                .HasForeignKey<Stock>(s => s.ProductId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Stock)
                .WithMany(s => s.Reservations)
                .HasForeignKey(s => s.StockId);

            modelBuilder.Entity<Stock>()
                .UseXminAsConcurrencyToken();
        }
    }
}
