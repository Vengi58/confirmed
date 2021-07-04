using ConfirmedAPI.Data;
using ConfirmedAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System;


namespace ConfirmedAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfirmedRepository repo;
        public ProductController(IConfirmedRepository confirmedRepository)
        {
            repo = confirmedRepository;
        }

        [HttpPatch("{id}/stock")]
        public ActionResult UpdateStock(int id, StockDTO stock)
        {
            try
            {
                var product = repo.GetProduct(id);

                if (product == null)
                    return NotFound("Product not found!");

                var stockOfProduct = repo.GetStockForProduct(product);

                if (stockOfProduct == null)
                    stockOfProduct = repo.CreateStockForProduct(product);

                stockOfProduct.InStock = stock.Stock;

                repo.UpdateStock(stockOfProduct);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<StockLevelDTO> GetStockLevel(int id)
        {
            try
            {
                var product = repo.GetProduct(id);
                if (product == null)
                    return NotFound("Product not found!");

                var stock = repo.GetStockForProduct(product);
                if (stock == null)
                    return NotFound("Stock not found for product!");

                return Ok(new StockLevelDTO { IN_STOCK = stock.InStock, RESERVED = stock.Reserved, SOLD = stock.Sold });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("{id}/reserve")]
        public ActionResult<ReservationDTO> Reserve(int id)
        {
            try
            {
                var res = repo.AddReservatonForProduct(id);
                if (res == null) return BadRequest();

                return Ok(new ReservationDTO { ReservationToken = res.Id.ToString() });
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Item out of stock!");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("{id}/unreserve")]
        public ActionResult<ReservationDTO> Unreserve(int id, ReservationDTO reservation)
        {
            try
            {
                var reservetionToken = Guid.Parse(reservation.ReservationToken);
                var res = repo.GetReservation(id, reservetionToken);
                if (res == null) return NotFound("Reservation not found!");
                repo.RemoveReservationForProduct(res);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("{id}/sold")]
        public ActionResult<ReservationDTO> Sold(int id, ReservationDTO reservation)
        {
            try
            {
                var reservetionToken = Guid.Parse(reservation.ReservationToken);
                var res = repo.GetReservation(id, reservetionToken);
                if (res == null) return NotFound("Reservation not found!");

                repo.SellReservedProduct(res);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
