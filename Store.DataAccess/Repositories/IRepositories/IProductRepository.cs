using Store.Models.Models;

namespace Store.DataAccess.Repositories.IRepositories;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}
