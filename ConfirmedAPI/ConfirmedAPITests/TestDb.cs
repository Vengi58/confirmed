using ConfirmedAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using ConfirmedAPI.Models;
using System.Linq;
using System;

namespace ConfirmedAPITests
{
    public class TestDb
    {
        public static Guid TestGuid = Guid.NewGuid();

        public static ConfirmedRepository CreateTestConfirmedRepository(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<ConfirmedDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ConfirmedDbContextTest(options);
            context.Database.EnsureCreated();

            context.Products.Add(new Product { Name = "Superstar" });
            context.Products.Add(new Product { Name = "Samba" });
            context.Products.Add(new Product { Name = "Stan Smith" });
            context.SaveChanges();

            var p1 = context.Products.FirstOrDefault(p => p.Name.Equals("Superstar"));
            var p2 = context.Products.FirstOrDefault(p => p.Name.Equals("Samba"));
            var p3 = context.Products.FirstOrDefault(p => p.Name.Equals("Stan Smith"));

            context.Stocks.Add(new Stock { InStock = 20, Reserved = 0, Sold = 0, Product = p1, ProductId = p1.ID });
            context.Stocks.Add(new Stock { InStock = 30, Reserved = 1, Sold = 0, Product = p2, ProductId = p2.ID });
            context.SaveChanges();

            var s1 = context.Stocks.FirstOrDefault(s => s.ProductId == p1.ID);
            var s2 = context.Stocks.FirstOrDefault(s => s.ProductId == p2.ID);


            context.Reservations.Add(new Reservation { Id = TestGuid, Product = p1, ProductId = p1.ID, Stock = s1, StockId = s1.ID  });
            context.SaveChanges();

            return new ConfirmedRepository(context);
        }
    }
}
