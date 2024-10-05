using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Product = new ProductRepository(context);
            Category = new CategoryRepository(context);
            OrderDetail = new OrderDetailRepository(context);
            Order = new OrderRepository(context);
            ShoppingCart = new ShoppingCartRepository(context);
        }

        public void Save()
        {
           context.SaveChanges();
        }
    }
}
