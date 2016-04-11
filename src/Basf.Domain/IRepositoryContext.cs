namespace Basf.Domain
{
    public interface IRepositoryContext : IUnitOfWork
    {
        IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class;
        IRepository<TAggRoot, TAggRootId> RepositoryFor<TAggRoot, TAggRootId>() where TAggRoot : class, IAggregateRoot<TAggRootId>;
    }
}
