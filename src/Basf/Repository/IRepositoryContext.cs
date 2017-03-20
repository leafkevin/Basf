using Basf.Domain;

namespace Basf.Repository
{
    public interface IRepositoryContext : IUnitOfWork
    {
        IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class;
    }
}
