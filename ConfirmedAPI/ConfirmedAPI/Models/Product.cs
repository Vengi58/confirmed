using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfirmedAPI.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; }

        public Stock Stock { get; set; }
    }
}
