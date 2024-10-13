using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models.Models;

namespace Store.DataAccess.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> categories { get; set; }
    public DbSet<OrderDetail> orderDetail { get; set; }
    public DbSet<Order> orders { get; set; }
    public DbSet<ShoppingCart> Shoppingcart { get; set; }
    public DbSet<Review> reviews { get; set; }
    public DbSet<ApplicationUser> applicationUsers { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
