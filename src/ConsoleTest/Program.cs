using Basf;
using Basf.Autofac;
using Basf.Log4net;
using Basf.LogMongo;
using Basf.Orm;
using Basf.Repository;
using System;
using System.Data.SqlClient;
using Basf.MongoStore;
using Basf.Domain.Repository;
using ConsoleTest.Domain.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ConsoleTest.Domain.ValueObject;

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
               //.UsingLogger<MongoLogger>()
               .UsingDapperRepository<SqlConnection, DefaultOrmProvider>("STS")
               .UsingMongoStore()
            );
            //泛型注册方式
            AppRuntime.RegisterGeneric(typeof(IRepository<>), typeof(Repository<>), LifetimeStyle.Request);
            //上面都是初始化注册

            //下面是具体使用



            //IRepositoryContext context = AppRuntime.Resolve<IRepositoryContext>();
            //IRepository<User> userRepository = context.RepositoryFor<User>();
            //IRepository<Order> orderRepository = context.RepositoryFor<Order>();
            //User user = userRepository.Get(new { UniqueId = 1 });
            //Order order = orderRepository.Get(new { UniqueId = 1 });
            //context.Begin();
            //userRepository.Update(user, new { UniqueId = 1 });
            //orderRepository.Update(order, new { UniqueId = 1 });
            //context.Commit();

            ////Q端使用
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
        private async Task ChangeInfo(int userId)
        {
            string commandId = Guid.NewGuid().ToString();
            IRepositoryContext context = AppRuntime.Resolve<IRepositoryContext>();
            IRepository<User> userRepository = context.RepositoryFor<User>();
            IRepository<Order> orderRepository = context.RepositoryFor<Order>();
            User user = userRepository.Get(new { UniqueId = userId });
            Order order = orderRepository.Get(new { UniqueId = 1 });
            await user.ChangeName(commandId, "kevin");
            await order.AddCommodity(commandId, new Goods(12, "goods", 35, 1));
            context.Begin();
            userRepository.Update(user, new { UniqueId = userId });
            orderRepository.Update(order, new { UniqueId = 1 });
            context.Commit();
        }
    }
}