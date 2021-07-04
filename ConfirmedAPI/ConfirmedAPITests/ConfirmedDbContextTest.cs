using ConfirmedAPI.Data;
using ConfirmedAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfirmedAPITests
{
    public class ConfirmedDbContextTest : ConfirmedDbContext
    {
        public ConfirmedDbContextTest(DbContextOptions<ConfirmedDbContext> options) : base(options) { }

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

            //this is different for SQLite, hence the inheritance
            modelBuilder.Entity<Stock>()
                    .Property(p => p.xmin)
                    .HasDefaultValue(1)
                    .IsRowVersion();
        }

    }
}
