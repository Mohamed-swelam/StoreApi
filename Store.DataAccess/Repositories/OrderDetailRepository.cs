using Store.DataAccess.Data;
using Store.Models.Models;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext context;

        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(OrderDetail OrderDetail)
        {
           context.orderDetail.Update(OrderDetail);
        }
    }
}
