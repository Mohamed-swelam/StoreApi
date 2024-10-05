using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Cart
{
    public class AddToCartDTO
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int productId { get; set; }
    }
}
