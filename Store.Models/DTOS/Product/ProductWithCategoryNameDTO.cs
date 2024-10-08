using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Product
{
    public class ProductWithCategoryNameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string? CategoryName { get; set; }
    }
}
