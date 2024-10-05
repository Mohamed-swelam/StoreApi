using Store.Models.Models;

namespace Store.DataAccess.Repositories.IRepositories;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart ShoppingCart);
}
