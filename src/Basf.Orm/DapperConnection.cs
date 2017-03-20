using System.Data;
using System.Data.Common;

namespace Basf.Orm
{
    public class DapperConnection : IDbConnection
    {
        private IDbConnection dbConnection;
        public IDbTransaction Transaction { get; set; }
        public string ConnectionString { get; set; }
        public int ConnectionTimeout
        {
            get { return this.dbConnection.ConnectionTimeout; }
        }
        public string Database
        {
            get { return this.dbConnection.Database; }
        }
        public ConnectionState State
        {
            get { return this.dbConnection.State; }
        }
        public DapperConnection(string connKey)
        {
            var provider = AppRuntime.Resolve<IOrmProvider>();
            this.dbConnection = provider.CreateConnection(connKey);
        }
        public void ChangeDatabase(string databaseName)
        {
            this.dbConnection.ChangeDatabase(databaseName);
        }
        public void Open()
        {
            if (this.dbConnection.State == ConnectionState.Broken)
            {
                this.dbConnection.Close();
            }
            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }
        }
        public IDbCommand CreateCommand()
        {
            return this.dbConnection.CreateCommand();
        }
        public IDbTransaction BeginTransaction()
        {
            return this.dbConnection.BeginTransaction();
        }
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return this.dbConnection.BeginTransaction(isolationLevel);
        }
        public void Close()
        {
            this.dbConnection.Close();
        }
        public void Dispose()
        {
            this.dbConnection.Dispose();
            this.dbConnection = null;
        }
        public static explicit operator DbConnection(DapperConnection dbConnection)
        {
            return (DbConnection)dbConnection.dbConnection;
        }
    }
}
