namespace Basf.Domain.Repository
{
    public interface IRepositoryContext : IUnitOfWork
    {
        IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class;
    }
}
