namespace Store.DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; }
        void Save();
    }
}
