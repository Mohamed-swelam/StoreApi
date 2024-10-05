using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Models
{
    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }
        public int Quantity {  get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        public Product Product { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
