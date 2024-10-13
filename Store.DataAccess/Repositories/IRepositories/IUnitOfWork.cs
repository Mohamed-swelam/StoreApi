using Store.Models.Models;

namespace Store.DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IRepository<Product> Product { get; }
        ICategoryRepository Category { get; }
        IRepository<ShoppingCart> ShoppingCart { get; }
        IRepository<OrderDetail> OrderDetail { get; }
        IOrderRepository Order { get; }
        IReviewRepository Review { get; }
        IUserRepository User { get; }
        void Save();
    }
}
