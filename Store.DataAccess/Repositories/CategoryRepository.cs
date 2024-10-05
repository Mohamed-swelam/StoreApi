using Store.DataAccess.Data;
using Store.Models.Models;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Category Category)
        {
           context.categories.Update(Category);
        }
    }
}
