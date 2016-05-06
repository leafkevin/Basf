namespace Basf.Domain.Repository
{
    public interface IRepositoryContext : IUnitOfWork
    {
        IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class;
        IRepository<TAggRoot, TAggRootId> RepositoryFor<TAggRoot, TAggRootId>() where TAggRoot : class, IAggRoot<TAggRootId>;
    }
}
