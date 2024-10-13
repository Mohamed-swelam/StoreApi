using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Review
{
    public class GetReviewDTO
    {
        public string Comment {  get; set; }
        public int Rating { get; set; }
        public string UserName { get; set; }
    }
}
