 using Basf.Domain;
using Basf.Domain.Repository;
using System.Data;

namespace Basf.DapperRepository
{
    public class RepositoryContext : IRepositoryContext
    {
        protected IDbConnection Connection { get; set; }
        protected IDbTransaction Transaction { get; set; }
        public RepositoryContext(IDbConnection connection)
        {
            this.Connection = connection;
        }
        public void Begin()
        {
            this.Transaction = this.Connection.BeginTransaction();
        }
        public void Commit()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
            }
        }
        /// <summary>
        /// 如果使用UnitOfWork的话，必须用此方法来获取实体的Repository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class
        {
            return AppRuntime.Resolve<IRepository<TEntity>>(this.Connection, this.Transaction);
        }
        /// <summary>
        /// 如果使用UnitOfWork的话，必须用此方法来获取聚合根的Repository
        /// </summary>
        /// <typeparam name="TAggRoot"></typeparam>
        /// <typeparam name="TAggRootId"></typeparam>
        /// <returns></returns>
        public IRepository<TAggRoot, TAggRootId> RepositoryFor<TAggRoot, TAggRootId>() where TAggRoot : class, IAggregateRoot<TAggRootId>
        {
            return AppRuntime.Resolve<IRepository<TAggRoot, TAggRootId>>(this.Connection, this.Transaction);
        }
        public void Rollback()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
            }
        }
        public void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
            }
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
        }
    }
}
