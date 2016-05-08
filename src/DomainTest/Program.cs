using Basf;
using Basf.Autofac;
using Basf.Domain.Repository;
using Basf.Log4net;
using Basf.MongoStore;
using Basf.Repository;
using Basf.Orm;
using DomainTest.Domain.Model;
using System.Data.SqlClient;
using System.Reflection;
using Basf.Domain;
using DomainTest.Command;
using System.Threading.Tasks;

namespace DomainTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BankAccount account1 = new BankAccount(1, 1, 100);
            BankAccount account2 = new BankAccount(2, 2, 100);

            //DDD领域 C端使用
            AppRuntime.Configure(f =>
              f.UsingContainer(new AutofacContainer())
               .UsingLogger<Log4NetLogger>()
               .UsingDapperRepository<SqlConnection, DefaultOrmProvider>("STS")
               .UsingMongoStore()
               .UsingDomain(Assembly.GetExecutingAssembly())
            );
        }
        public static async Task TestBankAccountTransfer()
        {
            var transferCommand = new BankAccountTransactionCommand(1, 2, 50);
            await AppRuntime.Resolve<ICommandContext>().ExecuteAsync(transferCommand);
        }
    }
}
