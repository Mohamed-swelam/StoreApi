using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Store.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address {  get; set; }
        public ICollection<Order> orders { get; set; }
    }
}
