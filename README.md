# Basf
一个DDD+CQRS+Event Sourcing的框架，正在搭建中，未成熟。
AppRuntime.Configure(f =>
              f.UsingContainer(new AutofacContainer())
               .UsingLogger<Log4NetLogger>()
               .UsingDapperRepository<SqlConnection, DefaultOrmProvider>("STS")
               .UsingMongoStore()
               .UsingDomain(Assembly.GetExecutingAssembly())
            );
            TestBankAccountTransfer().Wait();
