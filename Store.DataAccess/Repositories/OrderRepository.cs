using Store.DataAccess.Data;
using Store.Models.Models;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Order Order)
        {
           context.orders.Update(Order);
        }
    }
}
