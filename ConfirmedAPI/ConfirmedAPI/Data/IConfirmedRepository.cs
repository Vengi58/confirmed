using ConfirmedAPI.Models;
using System;

namespace ConfirmedAPI.Data
{
    public interface IConfirmedRepository
    {
        public Product GetProduct(int productId);
        public Stock GetStockForProduct(Product product);
        public Stock CreateStockForProduct(Product product);
        public void UpdateStock(Stock stock);
        public Reservation GetReservation(int productId, Guid reservationID);
        public Reservation AddReservatonForProduct(int productId);
        public void RemoveReservationForProduct(Reservation reservation);
        public void SellReservedProduct(Reservation reservation);
    }
}
