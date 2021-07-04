using ConfirmedAPI.Controllers;
using ConfirmedAPI.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.Net;

namespace ConfirmedAPITests
{
    public class ProductControllerTests
    {
        private ProductController productController;
        private SqliteConnection connection;
        [SetUp]
        public void Setup()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var repo = TestDb.CreateTestConfirmedRepository(connection);
            productController = new ProductController(repo);
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
        }

        [Test]
        public void GetStockLevel_valid_productId_should_return_stocklevel()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var result = stockLevel.Result as OkObjectResult;
            var stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);
        }

        [Test]
        public void GetStockLevel_invalid_productId_should_return_notfound()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(10);
            stockLevel.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public void GetStockLevel_non_existing_stock_should_return_notfound()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(3);
            stockLevel.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public void UpdateStock_valid_productId_should_update_stock_level()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var result = stockLevel.Result as OkObjectResult;
            var stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);

            var stockAfterUpdate = productController.UpdateStock(1, new StockDTO { Stock = 30 });
            stockAfterUpdate.Should().BeOfType<OkResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var stockLevelAfterUpdate = productController.GetStockLevel(1);
            Assert.NotNull(stockLevelAfterUpdate);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result = stockLevelAfterUpdate.Result as OkObjectResult;
            stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(30, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);
        }

        [Test]
        public void Reserve_valid_productId_should_reserve_and_update_stock_level()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var result = stockLevel.Result as OkObjectResult;
            var stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);


            var reservation = productController.Reserve(1);


            Assert.NotNull(reservation);
            reservation.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var reservationResult = reservation.Result as OkObjectResult;
            var reservationResultDTO = (reservationResult.Value as ReservationDTO);

            Assert.NotNull(reservationResultDTO);
            Assert.NotNull(reservationResultDTO.ReservationToken);

            stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result = stockLevel.Result as OkObjectResult;
            stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(19, stockLevelResult.IN_STOCK);
            Assert.AreEqual(2, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);
        }

        [Test]
        public void Reserve_invalid_productId_should_return_bad_request()
        {
            Assert.NotNull(productController);
            var reservation = productController.Reserve(4);
            reservation.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void Unreserve_valid_productId_should_unreserve_and_update_stock_level()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var result = stockLevel.Result as OkObjectResult;
            var stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);


            var reservation = productController.Unreserve(1, new ReservationDTO { ReservationToken = TestDb.TestGuid.ToString() });


            Assert.NotNull(reservation);
            reservation.Result.Should().BeOfType<OkResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result = stockLevel.Result as OkObjectResult;
            stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(21, stockLevelResult.IN_STOCK);
            Assert.AreEqual(0, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);
        }

        [Test]
        public void Unreserve_invalid_productId_should_return_notfound()
        {
            Assert.NotNull(productController);
            var reservation = productController.Unreserve(4, new ReservationDTO { ReservationToken = TestDb.TestGuid.ToString() });
            reservation.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public void Unreserve_invalid_token_should_return_notfound()
        {
            Assert.NotNull(productController);
            var reservation = productController.Unreserve(1, new ReservationDTO { ReservationToken = Guid.NewGuid().ToString() });
            reservation.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public void Sold_valid_productId_should_sell_and_update_stock_level()
        {
            Assert.NotNull(productController);
            var stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var result = stockLevel.Result as OkObjectResult;
            var stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(1, stockLevelResult.RESERVED);
            Assert.AreEqual(0, stockLevelResult.SOLD);


            var sold = productController.Sold(1, new ReservationDTO { ReservationToken = TestDb.TestGuid.ToString() });


            Assert.NotNull(sold);
            sold.Result.Should().BeOfType<OkResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            stockLevel = productController.GetStockLevel(1);
            Assert.NotNull(stockLevel);
            stockLevel.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result = stockLevel.Result as OkObjectResult;
            stockLevelResult = (result.Value as StockLevelDTO);
            Assert.AreEqual(20, stockLevelResult.IN_STOCK);
            Assert.AreEqual(0, stockLevelResult.RESERVED);
            Assert.AreEqual(1, stockLevelResult.SOLD);
        }

        [Test]
        public void Sold_invalid_productId_should_return_notfound()
        {
            Assert.NotNull(productController);
            var sold = productController.Sold(4, new ReservationDTO { ReservationToken = TestDb.TestGuid.ToString() });
            sold.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public void Sold_invalid_token_should_return_notfound()
        {
            Assert.NotNull(productController);
            var sold = productController.Sold(1, new ReservationDTO { ReservationToken = Guid.NewGuid().ToString() });
            sold.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

    }
}
