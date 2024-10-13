using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Review
{
    public class ReviewDTO
    {
        [Required]
        public string Comment {  get; set; }
        [Required]
        public int ProductId {  get; set; }
        [Range(1,5)]
        public int Rating { get; set; }
    }
}
