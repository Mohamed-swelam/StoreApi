using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Product
{
    public class ProductFilteringDTO
    {
        public string? CategoryName { get; set; } = null;
        public double? minPrice { get; set; } = 0;
        public double? maxPrice { get; set;} = 0;
        public string? KeyWord { get; set; } = null;
    }
}
