using Basf;
using Basf.Autofac;
using Basf.Log4net;
using Basf.Domain.Repository;
using Basf.DapperRepository;
using Basf.Orm;
using System;
using System.Data.SqlClient;
using Basf.LogMongo;
using ConsoleTest.Domain.Model;
using Basf.MongoStore;
using System.Collections.Generic;
using System.Data;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //DDD领域 C端使用
            AppRuntime.Configure(f =>
              f.UsingContainer(new AutofacContainer())
               .UsingLogger<Log4NetLogger>()
               .UsingLogger<MongoLogger>()
               .UsingRepository<SqlConnection, DefaultOrmProvider>("STS")
               .UsingMongoStore()
            );
            User user = AppRuntime.Resolve<IRepository<User>>().Get(new { UserId = 1 });

            //Q端使用
            //AppRuntime.Configure(f =>
            // f.UsingContainer(new AutofacContainer())
            //  .UsingLogger<Log4NetLogger>()
            //  .UsingLogger<MongoLogger>()
            //  .UsingOrmDapper<SqlConnection, DefaultOrmProvider>("STS")
            //);
            //List<User> userList = AppRuntime.Resolve<IDbConnection>().Query<User>("SELECT * FROM [User] WHERE UserId=@UserId", new { UserId = 1 }).AsList();
            //User user = AppRuntime.Resolve<IDbConnection>().Get<User>(new { UserId = 1 });

            AppRuntime.Debug("Test");
            Console.ReadLine();
        }
    }
}