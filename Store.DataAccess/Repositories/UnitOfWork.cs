using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.Models;

namespace Store.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public IRepository<Product> Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IRepository<OrderDetail> OrderDetail { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IRepository<ShoppingCart> ShoppingCart { get; private set; }
        public IReviewRepository Review { get; private set; }
        public IUserRepository User { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Product = new Repository<Product>(context);
            Category = new CategoryRepository(context);
            OrderDetail = new Repository<OrderDetail>(context);
            Order = new OrderRepository(context);
            ShoppingCart = new Repository<ShoppingCart>(context);
            Review = new ReviewRepository(context);
            User = new UserRepository(context);
        }

        public void Save()
        {
           context.SaveChanges();
        }
    }
}
