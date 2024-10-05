using Store.Models.Models;

namespace Store.DataAccess.Repositories.IRepositories;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category Category);
}
