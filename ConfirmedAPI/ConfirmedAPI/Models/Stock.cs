using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfirmedAPI.Models
{
    public class Stock
    {
        [Key]
        public int ID { get; set; }
        public uint xmin { get; set; }
        public int InStock { get; set; }
        public int Reserved { get; set; }
        public int Sold { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
