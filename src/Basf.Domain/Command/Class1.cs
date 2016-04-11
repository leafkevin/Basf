using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class DefaultDomainContext : ICommandContext
    {
        private IRepository _repository;

        public DefaultDomainContext()
        {
            this._repository = IoC.Resolve<IRepository>();
        }

        public void Add<T>(T domain) where T : class, IAggregateRoot
        {
            this._repository.Add<T>(domain);
        }

        public T Get<T>(int id) where T : class, IAggregateRoot
        {
            return this._repository.FindById<T>(id);
        }
    }
    public interface ICommandContext
    {
        void Add<TAggRoot, TAggRootId>(TAggRoot aggRoot) where TAggRoot : class, IAggregateRoot<TAggRootId>;

        TAggRoot Get<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggregateRoot<TAggRootId>;
    }
}
