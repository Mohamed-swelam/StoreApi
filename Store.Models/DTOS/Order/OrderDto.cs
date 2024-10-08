using Store.Models.DTOS.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.DTOS.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public List<Orderdetaildto>? OrderDetails { get; set; } = new();
    }
}
