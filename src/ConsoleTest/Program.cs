using Basf;
using Basf.Autofac;
using Basf.Log4net;
using Basf.Domain.Repository;
using Basf.DapperRepository;
using Basf.Orm;
using System;
using System.Data.SqlClient;
using Basf.LogMongo;
using System.Data;
using ConsoleTest.Domain.Model;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppRuntime.Configure(f =>
              f.UsingContainer(new AutofacContainer())
               .UsingLogger<Log4NetLogger>()
               .UsingLogger<MongoLogger>()
               .UsingRepository<SqlConnection, DefaultOrmProvider>("STS")
            );
            //AppRuntime.Configure(f =>
            // f.UsingContainer(new AutofacContainer())
            //  .UsingLogger<Log4NetLogger>()
            //  .UsingLogger<MongoLogger>()
            //  .UsingOrmDapper<SqlConnection, DefaultOrmProvider>("STS")
            //);
            //AppRuntime.Resolve<IDbConnection>().Query<Order>("SELECT COUNT(*) FORM ORDER");
            AppRuntime.Debug("Test");
            //List<User> userList = AppRuntime.Resolve<IDbConnection>().Query<User>("SELECT * FROM [User] WHERE UserId=@UserId", new { UserId = 1 }).AsList();
            //User user = AppRuntime.Resolve<IDbConnection>().Get<User>(new { UserId = 1 });
            User user = AppRuntime.Resolve<IRepository<User>>().Get(new { UserId = 1 });
            Console.ReadLine();
        }
    }
}