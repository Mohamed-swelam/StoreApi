using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS
{
    public class ProductTobeaddedDTO
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public int CategoryId {  get; set; }
    }
}
