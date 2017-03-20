using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Basf.Orm
{
    public class MyDapper
    {
        private string ConnKey { get; set; }
        private IDbTransaction transaction { get; set; }
        private IDbConnection connection { get; set; }
        private static Dictionary<string, string> sqlCache = new Dictionary<string, string>();
        public TEntity Get<TEntity>(string cacheKey, object condition) where TEntity : class
        {
            if (!sqlCache.ContainsKey(this.ConnKey + "-" + cacheKey))
            {
                //
            }
            var command = this.connection.CreateCommand();
            return connection.QueryFirstOrDefault<TEntity>(sqlCache[this.ConnKey + "-" + cacheKey], condition);
        }
        private void Open()
        {

        }
    }
}
