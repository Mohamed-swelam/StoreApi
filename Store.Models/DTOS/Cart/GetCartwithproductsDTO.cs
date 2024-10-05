using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Cart
{
    public class GetCartwithproductsDTO
    {
        public string ProductName {  get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
    }
}
