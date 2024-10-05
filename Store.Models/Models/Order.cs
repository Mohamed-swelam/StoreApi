using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId {  get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string OrderStatus {  get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
