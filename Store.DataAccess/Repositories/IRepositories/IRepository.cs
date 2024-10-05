using System.Linq.Expressions;

namespace Store.DataAccess.Repositories.IRepositories;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(Expression<Func<T,bool>>? expression = null,string? IncludeProperites = null);
    T Get(Expression<Func<T, bool>> expression, string? IncludeProperites = null);
    T GetById(int id);

    void Add(T entity);

    void Delete(T entity);
    void RemoveRange(List<T> entities);
    
}
