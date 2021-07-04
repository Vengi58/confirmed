using ConfirmedAPI.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConfirmedAPI.Data
{
    public class ConfirmedRepository : IConfirmedRepository
    {
        ConfirmedDbContext DBContext;
        public ConfirmedRepository(ConfirmedDbContext dBContext)
        {
            DBContext = dBContext;
        }
        public Reservation AddReservatonForProduct(int productId)
        {
            var product = DBContext.Products.FirstOrDefault(p => p.ID == productId);
            if (product == null) return null;

            bool savedSuccesfully = false;

            while (!savedSuccesfully)
            {
                var stock = DBContext.Stocks.FirstOrDefault(s => s.ProductId == productId);
                if (stock == null) return null;

                if (stock.InStock < 1) throw new ArgumentOutOfRangeException("Out of stock!");
                stock.InStock--;
                stock.Reserved++;

                DBContext.Reservations.Add(new Reservation { Product = product, ProductId = productId, Stock = stock, StockId = stock.ID, Id = Guid.NewGuid() });
                DBContext.Stocks.Update(stock);
                try
                {
                    DBContext.SaveChanges();
                    savedSuccesfully = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    DBContext.Reservations.Local.Clear();
                    DBContext.Stocks.Local.Clear();
                    DBContext.Entry(stock).Reload();
                    savedSuccesfully = false;
                }
            }
            return DBContext.Reservations.Local.First();
        }

        public Stock CreateStockForProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            DBContext.Stocks.Add(new Stock { Product = product, ProductId = product.ID  });
            DBContext.SaveChanges();
            return DBContext.Stocks.Local.First();
        }

        public Product GetProduct(int productId)
        {
            return DBContext.Products.FirstOrDefault(p => p.ID == productId);
        }

        public Stock GetStockForProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            return DBContext.Stocks.Include(s => s.Product).FirstOrDefault(s => s.ProductId == product.ID);
        }
        public Reservation GetReservation(int productId, Guid reservationID)
        {
            return DBContext.Reservations.FirstOrDefault(r => r.Id == reservationID && r.ProductId == productId);
        }

        public void RemoveReservationForProduct(Reservation reservation)
        {
            RetryReservationUpdate(reservation, r => r+1, s => s-1);
        }

        public void SellReservedProduct(Reservation reservation)
        {
            RetryReservationUpdate(reservation, r => r-1, s => s+1);
        }

        private void RetryReservationUpdate(Reservation reservation, Func<int,int> reservedFun, Func<int, int> soldFun)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));

            var product = GetProduct(reservation.ProductId);

            bool savedSuccesfully = false;

            while (!savedSuccesfully || DBContext.Reservations.FirstOrDefault(r => r.Id == reservation.Id) != null)
            {
                var stock = GetStockForProduct(product);

                stock.Reserved = reservedFun(stock.Reserved);
                stock.InStock = soldFun(stock.InStock);

                DBContext.Reservations.Remove(reservation);
                DBContext.Stocks.Update(stock);
                try
                {
                    DBContext.SaveChanges();
                    savedSuccesfully = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    DBContext.Reservations.Add(reservation);
                    DBContext.Entry(stock).Reload();
                    savedSuccesfully = false;
                }
            }
        }

        public void UpdateStock(Stock stock)
        {
            if (stock == null) throw new ArgumentNullException(nameof(stock));

            DBContext.Stocks.Update(stock);
            DBContext.SaveChanges();
        }
    }
}
