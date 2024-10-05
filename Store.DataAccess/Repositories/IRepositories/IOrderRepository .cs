using Store.Models.Models;

namespace Store.DataAccess.Repositories.IRepositories;

public interface IOrderRepository : IRepository<Order>
{
    void Update(Order Order);
}
