using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.IRepositories;

namespace Store.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext context;
    internal DbSet<T> dbset;

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
        this.dbset = context.Set<T>();
    }
    public void Add(T entity)
    {
        dbset.Add(entity);
    }

    public void Delete(T entity)
    {
        dbset?.Remove(entity);
    }

    public T Get(Expression<Func<T, bool>> expression, string? IncludeProperites = null)
    {
        IQueryable<T> entity = dbset;
        if (!string.IsNullOrEmpty(IncludeProperites))
        {
            foreach (var item in IncludeProperites.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                entity = entity.Include(item);
            }
        }
        T? value = entity.FirstOrDefault(expression);
        return value;
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? IncludeProperites = null)
    {
        IQueryable<T> values = dbset;
        if (expression != null)
        {
            values = values.Where(expression);
        }
        if (!string.IsNullOrEmpty(IncludeProperites))
        {
            foreach (var item in IncludeProperites.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                values = values.Include(item);
            }
        }
        return values.ToList();
    }

    public T GetById(int id)
    {
        T? entity = dbset.Find(id);
        return entity;
    }

    public void RemoveRange(List<T> entities)
    {
        dbset.RemoveRange(entities);
    }
}
