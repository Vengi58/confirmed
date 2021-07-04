using ConfirmedAPI.Data;
using ConfirmedAPI.Models;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;

namespace ConfirmedAPITests
{
    public class Tests
    {
        private SqliteConnection connection;
        private ConfirmedRepository repo;

        [SetUp]
        public void Setup()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            repo = TestDb.CreateTestConfirmedRepository(connection);
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
        }

        [Test]
        public void GetProduct_valid_productId_should_return_product()
        {
            var product = repo.GetProduct(1);

            Assert.NotNull(product);
            Assert.AreEqual(1, product.ID);
            Assert.AreEqual("Superstar", product.Name);
        }

        [Test]
        public void GetProduct_invalid_productId_should_return_null()
        {
            var product = repo.GetProduct(20);

            Assert.Null(product);
        }

        [Test]
        public void GetReservation_valid_productId_should_return_reservation()
        {
            var reservation = repo.GetReservation(1, TestDb.TestGuid);

            Assert.NotNull(reservation);
            Assert.AreEqual(1, reservation.ProductId);
        }

        [Test]
        public void GetReservation_invalid_productId_should_return_null()
        {
            var reservation = repo.GetReservation(10, TestDb.TestGuid);

            Assert.Null(reservation);
        }

        [Test]
        public void GetReservation_invalid_token_should_return_null()
        {
            var reservation = repo.GetReservation(1, Guid.NewGuid());

            Assert.Null(reservation);
        }

        [Test]
        public void GetStockForProduct_valid_product_should_return_stock()
        {
            var product = repo.GetProduct(1);
            var stock = repo.GetStockForProduct(product);

            Assert.NotNull(stock);
            Assert.NotNull(stock.Product);
            Assert.AreEqual(stock.ProductId, product.ID);
            Assert.AreEqual(stock.Product, product);
        }

        [Test]
        public void GetStockForProduct_invalid_product_should_return_null()
        {
            var product = new Product { ID = 0, Name = "Gazelle", Stock = null };
            var stock = repo.GetStockForProduct(product);

            Assert.Null(stock);
        }

        [Test]
        public void GetStockForProduct_null_product_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => repo.GetStockForProduct(null));
        }

        [Test]
        public void CreateStockForProduct_valid_product_should_return_stock()
        {
            var product = repo.GetProduct(3);
            var stock = repo.CreateStockForProduct(product);

            Assert.NotNull(stock);
            Assert.NotNull(stock.Product);
            Assert.AreEqual(stock.ProductId, product.ID);
            Assert.AreEqual(stock.Product, product);
        }

        [Test]
        public void CreateStockForProduct_invalid_product_should_return_stock_with_product_created()
        {
            var product = new Product { Name = "Gazelle", Stock = null };
            var stock = repo.CreateStockForProduct(product);
            var product0 = repo.GetProduct(0);

            Assert.NotNull(stock);
            Assert.NotNull(stock.Product);
            Assert.AreEqual(stock.ProductId, product.ID);
            Assert.AreEqual(stock.Product, product);
        }

        [Test]
        public void CreateStockForProduct_null_product_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => repo.CreateStockForProduct(null));
        }

        [Test]
        public void AddReservatonForProduct_valid_productid_should_return_reservation()
        {
            var product = repo.GetProduct(1);

            var stockBeforeReservation = repo.GetStockForProduct(product);
            var InStockBeforeReservation = stockBeforeReservation.InStock;
            var ReservedkBeforeReservation = stockBeforeReservation.Reserved;
            var SoldBeforeReservation = stockBeforeReservation.Sold;

            var reservation = repo.AddReservatonForProduct(1);

            var stockAfterReservation = repo.GetStockForProduct(product);
            var InStockAfterReservation = stockAfterReservation.InStock;
            var ReservedkAfterReservation = stockAfterReservation.Reserved;
            var SoldAfterReservation = stockAfterReservation.Sold;


            Assert.NotNull(reservation);
            Assert.AreEqual(reservation.ProductId, 1);
            Assert.AreEqual(InStockBeforeReservation -1 , InStockAfterReservation);
            Assert.AreEqual(ReservedkBeforeReservation + 1, ReservedkAfterReservation);
            Assert.AreEqual(SoldBeforeReservation, SoldAfterReservation);
        }

        [Test]
        public void AddReservatonForProduct_invalid_productid_should_return_null()
        {
            var reservation = repo.AddReservatonForProduct(10);

            Assert.Null(reservation);
        }

        [Test]
        public void RemoveReservationForProduct_valid_productid_should_remove_reservation()
        {
            var product = repo.GetProduct(1);
            var stockBeforeReservation = repo.GetStockForProduct(product);
            var InStockBeforeReservation = stockBeforeReservation.InStock;
            var ReservedkBeforeReservation = stockBeforeReservation.Reserved;
            var SoldBeforeReservation = stockBeforeReservation.Sold;

            var reservation = repo.AddReservatonForProduct(1);

            var stockAfterReservation = repo.GetStockForProduct(product);
            var InStockAfterReservation = stockAfterReservation.InStock;
            var ReservedkAfterReservation = stockAfterReservation.Reserved;
            var SoldAfterReservation = stockAfterReservation.Sold;

            repo.RemoveReservationForProduct(reservation);

            var stockAfterRemoveReservation = repo.GetStockForProduct(product);
            var InStockAfterRemoveReservation = stockAfterRemoveReservation.InStock;
            var ReservedkAfterRemoveReservation = stockAfterRemoveReservation.Reserved;
            var SoldAfterRemoveReservation = stockAfterRemoveReservation.Sold;

            var reservationRemoved = repo.GetReservation(1, reservation.Id);

            Assert.NotNull(reservation);
            Assert.Null(reservationRemoved);
            Assert.AreEqual(InStockBeforeReservation - 1, InStockAfterReservation);
            Assert.AreEqual(ReservedkBeforeReservation + 1, ReservedkAfterReservation);
            Assert.AreEqual(SoldBeforeReservation, SoldAfterReservation);
            Assert.AreEqual(InStockAfterReservation + 1, InStockAfterRemoveReservation);
            Assert.AreEqual(ReservedkAfterReservation - 1, ReservedkAfterRemoveReservation);
            Assert.AreEqual(SoldAfterReservation, SoldAfterRemoveReservation);
        }

        [Test]
        public void SellReservedProduct_valid_productid_should_remove_reservation()
        {
            var product = repo.GetProduct(1);
            var stockBeforeReservation = repo.GetStockForProduct(product);
            var InStockBeforeReservation = stockBeforeReservation.InStock;
            var ReservedkBeforeReservation = stockBeforeReservation.Reserved;
            var SoldBeforeReservation = stockBeforeReservation.Sold;

            var reservation = repo.AddReservatonForProduct(1);

            var stockAfterReservation = repo.GetStockForProduct(product);
            var InStockAfterReservation = stockAfterReservation.InStock;
            var ReservedkAfterReservation = stockAfterReservation.Reserved;
            var SoldAfterReservation = stockAfterReservation.Sold;

            repo.SellReservedProduct(reservation);

            var stockAfterSell = repo.GetStockForProduct(product);
            var InStockAfterSell = stockAfterSell.InStock;
            var ReservedkAfterSell = stockAfterSell.Reserved;
            var SoldAfterSell = stockAfterSell.Sold;

            var reservationRemoved = repo.GetReservation(1, reservation.Id);

            Assert.NotNull(reservation);
            Assert.Null(reservationRemoved);
            Assert.AreEqual(InStockBeforeReservation - 1, InStockAfterReservation);
            Assert.AreEqual(ReservedkBeforeReservation + 1, ReservedkAfterReservation);
            Assert.AreEqual(SoldBeforeReservation, SoldAfterReservation);
            Assert.AreEqual(InStockAfterReservation, InStockAfterSell);
            Assert.AreEqual(ReservedkAfterReservation - 1, ReservedkAfterSell);
            Assert.AreEqual(SoldAfterReservation + 1, SoldAfterSell);
        }

        [Test]
        public void UpdateStock_invalid_product_should_return_stock_with_product_created()
        {
            var product = repo.GetProduct(1);
            var stock = repo.GetStockForProduct(product);
            stock.InStock = 120;
            repo.UpdateStock(stock);
            var stockAfterUpdate = repo.GetStockForProduct(product);

            Assert.NotNull(stockAfterUpdate);
            Assert.NotNull(stockAfterUpdate.Product);
            Assert.AreEqual(stockAfterUpdate.ProductId, product.ID);
            Assert.AreEqual(stockAfterUpdate.InStock, 120);
        }
    }
}