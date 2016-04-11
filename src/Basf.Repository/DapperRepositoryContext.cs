using Basf.Domain;
using System.Data;

namespace Basf.Repository
{
    public class DapperRepositoryContext : IRepositoryContext
    {
        protected IDbConnection Connection { get; set; }
        protected IDbTransaction Transaction { get; set; }
        public DapperRepositoryContext(IDbConnection connection)
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
        public IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : class
        {
            return new DapperRepository<TEntity>(this.Connection, this.Transaction);
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
