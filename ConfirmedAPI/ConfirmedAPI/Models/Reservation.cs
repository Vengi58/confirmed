using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConfirmedAPI.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int StockId { get; set; }
        public Stock Stock { get; set; }

    }
}
