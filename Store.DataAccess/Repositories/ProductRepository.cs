using Store.DataAccess.Data;
using Store.Models.Models;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Product product)
        {
           context.Products.Update(product);
        }
    }
}
