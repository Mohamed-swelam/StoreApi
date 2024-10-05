using Store.DataAccess.Data;
using Store.Models.Models;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(ShoppingCart ShoppingCart)
        {
           context.Shoppingcart.Update(ShoppingCart);
        }
    }
}
